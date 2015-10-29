using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kentor.AuthServices.Internal;
using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.Saml2P;
using System.IdentityModel.Configuration;
using System.IdentityModel.Services.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.CodeAnalysis;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config section for the module.
    /// </summary>
    public class KentorAuthServicesSection : ConfigurationSection, ISPOptions
    {
        private static readonly KentorAuthServicesSection current =
            (KentorAuthServicesSection)ConfigurationManager.GetSection("kentor.authServices");

        private bool allowChange = true;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "allowChange")]
        internal void AllowChange(bool allowChange)
        {
            this.allowChange = allowChange;
        }

        /// <summary>
        /// Used for testing, always returns true in production.
        /// </summary>
        /// <returns>Returns true (unless during tests)</returns>
        public override bool IsReadOnly()
        {
            return !allowChange;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public KentorAuthServicesSection()
        {
            saml2PSecurityTokenHandler = new Lazy<Saml2PSecurityTokenHandler>(
                () => new Saml2PSecurityTokenHandler(this),
                true);
        }

        /// <summary>
        /// Current config as read from app/web.config.
        /// </summary>
        public static KentorAuthServicesSection Current
        {
            get
            {
                return current;
            }
        }

        /// <summary>
        /// EntityId - The identity of the ServiceProvider to use when sending requests to Idp
        /// and presenting the SP in metadata.
        /// </summary>
        [TypeConverter(typeof(EntityIdConverter))]
        [ConfigurationProperty("entityId")]
        public EntityId EntityId
        {
            get
            {
                return (EntityId)base["entityId"];
            }
        }

        /// <summary>
        /// The Url to redirect back to after successfull authentication.
        /// </summary>
        [ConfigurationProperty("returnUrl")]
        public Uri ReturnUrl
        {
            get
            {
                return (Uri)base["returnUrl"];
            }
        }

        /// <summary>
        /// For how long may the metadata be cached by a receiving party?
        /// </summary>
        public TimeSpan MetadataCacheDuration
        {
            get
            {
                return Metadata.CacheDuration;
            }
        }

        /// <summary>
        /// Set of identity providers known to the service provider.
        /// </summary>
        [ConfigurationProperty("identityProviders")]
        [ConfigurationCollection(typeof(IdentityProviderCollection))]
        public IdentityProviderCollection IdentityProviders
        {
            get
            {
                return (IdentityProviderCollection)base["identityProviders"];
            }
        }

        /// <summary>
        /// Set of federations. The service provider will trust all the idps in these federations.
        /// </summary>
        [ConfigurationProperty("federations")]
        [ConfigurationCollection(typeof(FederationCollection))]
        public FederationCollection Federations
        {
            get
            {
                return (FederationCollection)base["federations"];
            }
        }

        const string discoveryServiceUrl = "discoveryServiceUrl";
        /// <summary>
        /// Url to discovery service to use if no idp is specified in the sign in call.
        /// </summary>
        [ConfigurationProperty(discoveryServiceUrl, IsRequired = false)]
        public Uri DiscoveryServiceUrl
        {
            get
            {
                return (Uri)base[discoveryServiceUrl];
            }
        }

        private readonly Lazy<Saml2PSecurityTokenHandler> saml2PSecurityTokenHandler;

        /// <summary>
        /// The security token handler used to process incoming assertions for this SP.
        /// </summary>
        public Saml2PSecurityTokenHandler Saml2PSecurityTokenHandler
        {
            get
            {
                return saml2PSecurityTokenHandler.Value;
            }
        }

        const string modulePath = "modulePath";
        /// <summary>
        /// Application root relative path for AuthServices endpoints. The 
        /// default is "AuthServices".
        /// </summary>
        [ConfigurationProperty(modulePath, IsRequired=false, DefaultValue="/AuthServices")]
        [RegexStringValidator("/.*")]
        public string ModulePath
        {
            get
            {
                return (string)base[modulePath];
            }
        }

        // Reset by the tests.
        internal Organization organization = null;

        /// <summary>
        /// Metadata describing the organization responsible for the entity.
        /// </summary>
        public Organization Organization
        {
            get
            {
                // If the entire organization element is missing in the config file,
                // Metadata.Organization will still be instantiated, but the Url will be null.
                if(organization == null && Metadata.Organization.Url != null)
                {
                    var culture = CultureInfo.InvariantCulture;
                    if(!string.IsNullOrEmpty(Metadata.Organization.Language))
                    {
                        culture = CultureInfo.GetCultureInfo(Metadata.Organization.Language);
                    }

                    var org = new Organization();
                    org.Names.Add(new LocalizedName(Metadata.Organization.Name, culture));
                    org.DisplayNames.Add(new LocalizedName(Metadata.Organization.DisplayName, culture));
                    org.Urls.Add(new LocalizedUri(Metadata.Organization.Url, culture));

                    organization = org;
                }

                return organization;
            }
        }

        const string metadata = "metadata";
        /// <summary>
        /// Metadata of the service provider.
        /// </summary>
        [ConfigurationProperty(metadata)]
        public MetadataElement Metadata
        {
            get
            {
                return (MetadataElement)base[metadata];
            }
            internal set
            {
                base[metadata] = value;
            }
        }

        IEnumerable<ContactPerson> contacts;

        /// <summary>
        /// Contacts for the SAML2 entity.
        /// </summary>
        public IEnumerable<ContactPerson> Contacts
        {
            get
            {
                if(contacts == null)
                {
                    // Won't assign directly to avoid a race condition.
                    var temp = new List<ContactPerson>();

                    foreach(var configPerson in Metadata.Contacts)
                    {
                        var contactPerson = new ContactPerson(configPerson.ContactType)
                        {
                            Company = configPerson.Company.NullIfEmpty(),
                            GivenName = configPerson.GivenName.NullIfEmpty(),
                            Surname = configPerson.Surname.NullIfEmpty(),
                        };

                        if(!string.IsNullOrEmpty(configPerson.PhoneNumber))
                        {
                            contactPerson.TelephoneNumbers.Add(configPerson.PhoneNumber);
                        }

                        if(!string.IsNullOrEmpty(configPerson.Email))
                        {
                            contactPerson.EmailAddresses.Add(configPerson.Email);
                        }

                        temp.Add(contactPerson);
                    }

                    contacts = temp;
                }

                return contacts;
            }
        }

        /// <summary>
        /// Attribute consuming services.
        /// </summary>
        public IEnumerable<AttributeConsumingService> AttributeConsumingServices
        {
            get
            {
                if (Metadata.RequestedAttributes.Any())
                {
                    var acs = new AttributeConsumingService("SP")
                    {
                        IsDefault = true
                    };

                    foreach (var confAttribute in Metadata.RequestedAttributes)
                    {
                        acs.RequestedAttributes.Add(new RequestedAttribute(confAttribute.Name)
                            {
                                FriendlyName = confAttribute.FriendlyName,
                                IsRequired = confAttribute.IsRequired,
                                NameFormat = confAttribute.NameFormat
                            });
                    }

                    yield return acs;
                }
            }
        }

        private IdentityConfiguration systemIdentityModelIdentityConfiguration
            = new IdentityConfiguration(true);

        /// <summary>
        /// The System.IdentityModel configuration to use.
        /// </summary>
        public IdentityConfiguration SystemIdentityModelIdentityConfiguration
        {
            get
            {
                return systemIdentityModelIdentityConfiguration;
            }
        }

        /// <summary>
        /// Certificate location for the certificate the Service Provider uses to decrypt assertions.
        /// </summary>
        [ConfigurationProperty("serviceCertificate")]
        [ExcludeFromCodeCoverage]
        public CertificateElement ServiceCertificateConfiguration
        {
            get
            {
                return (CertificateElement)base["serviceCertificate"];
            }
            internal set
            {
                base["serviceCertificate"] = value;
            }
        }

        /// <summary>
        /// Certificate for service provider to use when decrypting assertions
        /// </summary>
        public X509Certificate2 ServiceCertificate { get; set; }
    }
}
