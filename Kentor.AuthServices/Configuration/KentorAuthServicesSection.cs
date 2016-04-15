using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Linq;
using Kentor.AuthServices.Internal;
using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.Saml2P;
using System.Diagnostics.CodeAnalysis;
using System.Collections.ObjectModel;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config section for the module.
    /// </summary>
    public class KentorAuthServicesSection : ConfigurationSection
    {
        private static readonly KentorAuthServicesSection current =
            (KentorAuthServicesSection)ConfigurationManager.GetSection("kentor.authServices");

        internal bool AllowChange { get; set; }

        /// <summary>
        /// Used for testing, always returns true in production.
        /// </summary>
        /// <returns>Returns true (unless during tests)</returns>
        public override bool IsReadOnly()
        {
            return !AllowChange;
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
        /// By default, the service provider uses the host, protocol, and port
        /// from the HTTP request when creating links. This might not be
        /// accurate in reverse proxy or load-balancing situations. You can
        /// override the origin used for link generation using this property.
        /// </summary>
        [ConfigurationProperty("publicOrigin", IsRequired = false)]
        public Uri PublicOrigin
        {
            get
            {
                return (Uri)base["publicOrigin"];
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

        const string nameIdPolicy = nameof(nameIdPolicy);
        /// <summary>
        /// NamedId policy element.
        /// </summary>
        [ConfigurationProperty(nameIdPolicy)]
        public NameIdPolicyElement NameIdPolicyElement
        {
            get
            {
                return (NameIdPolicyElement) base[nameIdPolicy];
            }
        }

        const string requestedAuthnContext = nameof(requestedAuthnContext);
        /// <summary>
        /// RequestedAuthnContext config.
        /// </summary>
        [ConfigurationProperty(requestedAuthnContext)]
        public RequestedAuthnContextElement RequestedAuthnContext
        {
            get
            {
                return (RequestedAuthnContextElement)base[requestedAuthnContext];
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

        const string serviceCertificates = nameof(serviceCertificates);
        /// <summary>
        /// Certificates used by the service provider for signing and/or decryption.
        /// </summary>
        [ConfigurationProperty(serviceCertificates)]
        [ConfigurationCollection(typeof(ServiceCertificateElementCollection))]
        public ServiceCertificateElementCollection ServiceCertificates
        {
            get
            {
                return (ServiceCertificateElementCollection)base[serviceCertificates];
            }
        }

        const string authenticateRequestSigningBehavior = nameof(authenticateRequestSigningBehavior);
        /// <summary>
        /// Signing behavior for created AuthnRequests.
        /// </summary>
        [ConfigurationProperty(authenticateRequestSigningBehavior)]
        public SigningBehavior AuthenticateRequestSigningBehavior
        {
            get
            {
                return (SigningBehavior)base[authenticateRequestSigningBehavior];
            }
            internal set
            {
                base[authenticateRequestSigningBehavior] = value;
            }
        }

        const string validateCertificates = nameof(validateCertificates);
        /// <summary>
        /// Validate certificates when validating signatures? Normally not a
        /// good idea as SAML2 deployments typically exchange certificates
        /// directly and isntead of relying on the public certificate
        /// infrastructure.
        /// </summary>
        [ConfigurationProperty(validateCertificates, IsRequired = false)]
        public bool ValidateCertificates
        {
            get
            {
                return (bool)base[validateCertificates];
            }
            internal set
            {
                base[validateCertificates] = value;
            }
        }

        const string compatibility = nameof(compatibility);

        /// <summary>
        /// Compatibility settings. Can be used to make AuthServices accept
        /// certain non-standard behaviour.
        /// </summary>
        [ConfigurationProperty(compatibility)]
        public CompatibilityElement Compatibility 
        {
            get
            {
                return (CompatibilityElement)base[compatibility];
            }
        }
    }
}
