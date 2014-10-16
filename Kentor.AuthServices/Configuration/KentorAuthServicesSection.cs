using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
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

        public KentorAuthServicesSection()
        {
            saml2PSecurityTokenHandler = new Lazy<Saml2PSecurityTokenHandler>(
                () => new Saml2PSecurityTokenHandler(new EntityId(EntityId)),
                true);
        }

        private bool isReadOnly = true;

        internal void AllowConfigEdit(bool allow)
        {
            isReadOnly = !allow;
        }

        /// <summary>
        /// Allows local modification of the configuration for testing purposes
        /// </summary>
        /// <returns></returns>
        public override bool IsReadOnly()
        {
            return isReadOnly;
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
        /// Uri for idp to post responses to.
        /// </summary>
        [ConfigurationProperty("assertionConsumerServiceUrl")]
        public Uri AssertionConsumerServiceUrl
        {
            get
            {
                return (Uri)base["assertionConsumerServiceUrl"];
            }
        }

        /// <summary>
        /// EntityId - The identity of the ServiceProvider to use when sending requests to Idp
        /// and presenting the SP in metadata.
        /// </summary>
        [ConfigurationProperty("entityId")]
        public string EntityId
        {
            get
            {
                return (string)base["entityId"];
            }
        }

        /// <summary>
        /// The Uri to redirect back to after successfull authentication.
        /// </summary>
        [ConfigurationProperty("returnUri", IsRequired=true)]
        public Uri ReturnUri
        {
            get
            {
                return (Uri)base["returnUri"];
            }
        }

        /// <summary>
        /// Optional attribute that describes for how long in seconds anyone may cache the metadata 
        /// presented by the service provider. Defaults to 3600 seconds.
        /// </summary>
        [ConfigurationProperty("metadataCacheDuration", IsRequired=false, DefaultValue="1:0:0")]
        public TimeSpan MetadataCacheDuration
        {
            get
            {
                return (TimeSpan)base["metadataCacheDuration"];
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
        /// Url to discovery service to use if now idp is specified in the sign in call.
        /// </summary>
        [ConfigurationProperty(discoveryServiceUrl, IsRequired=false)]
        public Uri DiscoveryServiceUrl
        {
            get
            {
                return (Uri)base[discoveryServiceUrl];
            }
            set
            {
                base[discoveryServiceUrl] = value;
            }
        }

        const string discoveryServiceResponseUrl = "discoveryServiceResponseUrl";
        /// <summary>
        /// Url where to receive discovery service responses.
        /// </summary>
        [ConfigurationProperty(discoveryServiceResponseUrl, IsRequired = false)]
        public Uri DiscoveryServiceResponseUrl
        {
            get
            {
                return (Uri)base[discoveryServiceResponseUrl];
            }
            set
            {
                base[discoveryServiceResponseUrl] = value;
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
    }
}
