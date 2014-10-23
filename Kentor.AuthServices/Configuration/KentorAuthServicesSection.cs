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

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config section for the module.
    /// </summary>
    public class KentorAuthServicesSection : ConfigurationSection, ISPOptions
    {
        private static readonly KentorAuthServicesSection current =
            (KentorAuthServicesSection)ConfigurationManager.GetSection("kentor.authServices");

        /// <summary>
        /// Ctor
        /// </summary>
        public KentorAuthServicesSection()
        {
            saml2PSecurityTokenHandler = new Lazy<Saml2PSecurityTokenHandler>(
                () => new Saml2PSecurityTokenHandler(EntityId),
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
        /// The Uri to redirect back to after successfull authentication.
        /// </summary>
        [ConfigurationProperty("returnUri", IsRequired = true)]
        public Uri ReturnUri
        {
            get
            {
                return (Uri)base["returnUri"];
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

        public IEnumerable<AttributeConsumingService> AttributeConsumingServices
        {
            get
            {
                var acs = new AttributeConsumingService("SP");

                foreach(var confAttribute in Metadata.RequestedAttributes)
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
}
