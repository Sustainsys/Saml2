using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Configuration;
using System.IdentityModel.Services.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// <c>FederationIdentityConfiguration</c> is used to load a FederationConfiguration directly from Web.Config without the need to use System.Web.
    /// </summary>
    public class FederationIdentityConfiguration : FederationConfiguration
    {
        /// <summary>
        /// Fills the <c>FederationIdentityConfiguration</c> from Web.Config/App.Config without the need to use System.Web. 
        /// </summary>
        public void FromConfiguration()
        {
            SystemIdentityModelServicesSection configurationSection =
                (SystemIdentityModelServicesSection)ConfigurationManager.GetSection("system.identityModel.services");

            LoadConfiguration((FederationConfigurationElement)configurationSection.FederationConfigurationElements.GetElement(""));            
        }
    }
}
