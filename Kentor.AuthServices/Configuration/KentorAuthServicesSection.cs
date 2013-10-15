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
        /// Issuer name of to use when sending requests to Idp.
        /// </summary>
        [ConfigurationProperty("issuer")]
        public string Issuer
        {
            get
            {
                return (string)base["issuer"];
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
        /// Set of identity providers known to the service provider.
        /// </summary>
        [ConfigurationProperty("identityProviders", IsRequired=true)]
        [ConfigurationCollection(typeof(IdentityProviderCollection), AddItemName="identityProvider")]
        public IdentityProviderCollection IdentityProviders
        {
            get
            {
                return (IdentityProviderCollection)base["identityProviders"];
            }
        }
    }
}
