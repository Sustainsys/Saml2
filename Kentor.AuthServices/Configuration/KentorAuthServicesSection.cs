using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        /// Url for idp to post responses to.
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
        /// Url to discovery service to use if no idp is specified in the sign in call.
        /// </summary>
        [ConfigurationProperty(discoveryServiceUrl, IsRequired=false)]
        public Uri DiscoveryServiceUrl
        {
            get
            {
                return (Uri)base[discoveryServiceUrl];
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
