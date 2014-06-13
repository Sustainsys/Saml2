using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config section for the module.
    /// </summary>
    public class KentorAuthServicesSection : ConfigurationSection
    {
        private static readonly KentorAuthServicesSection current = 
            (KentorAuthServicesSection)ConfigurationManager.GetSection("kentor.authServices");

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
        /// EntityId - the name of the ServiceProvider to use when sending requests to Idp.
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
        [ConfigurationProperty("metadataCacheDuration", IsRequired=false, DefaultValue=3600)]
        public int MetadataCacheDuration
        {
            get
            {
                return (int)base["metadataCacheDuration"];
            }
        }

        /// <summary>
        /// Set of identity providers known to the service provider.
        /// </summary>
        [ConfigurationProperty("", IsRequired=true, IsDefaultCollection=true)]
        [ConfigurationCollection(typeof(IdentityProviderCollection), AddItemName="identityProvider")]
        public IdentityProviderCollection IdentityProviders
        {
            get
            {
                return (IdentityProviderCollection)base[""];
            }
        }
    }
}
