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
    public class Saml2AuthenticationModuleSection : ConfigurationSection
    {
        private static readonly Saml2AuthenticationModuleSection current = 
            (Saml2AuthenticationModuleSection)ConfigurationManager.GetSection("saml2AuthenticationModule");

        /// <summary>
        /// Current config as read from app/web.config.
        /// </summary>
        public static Saml2AuthenticationModuleSection Current
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
        /// Set of identity providers known to the service provider.
        /// </summary>
        [ConfigurationProperty("identityProviders", IsRequired=true)]
        [ConfigurationCollection(typeof(IdentityProviderCollection))]
        public IdentityProviderCollection IdentityProviders
        {
            get
            {
                return (IdentityProviderCollection)base["identityProviders"];
            }
        }
    }
}
