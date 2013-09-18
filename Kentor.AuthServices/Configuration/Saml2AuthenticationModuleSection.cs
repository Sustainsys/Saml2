using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    public class Saml2AuthenticationModuleSection : ConfigurationSection
    {
        private static readonly Saml2AuthenticationModuleSection current = 
            (Saml2AuthenticationModuleSection)ConfigurationManager.GetSection("saml2AuthenticationModule");

        public static Saml2AuthenticationModuleSection Current
        {
            get
            {
                return current;
            }
        }

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
