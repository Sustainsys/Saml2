using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Configuration of a federation.
    /// </summary>
    public class FederationElement : ConfigurationElement
    {
        private const string metadataUrl = "metadataUrl";
        /// <summary>
        /// Url to download metdata for the federation from.
        /// </summary>
        [ConfigurationProperty(metadataUrl, IsRequired = true)]
        public Uri MetadataUrl
        {
            get
            {
                return (Uri)base[metadataUrl];
            }
        }

        private const string allowUnsolicitedAuthnResponse = "allowUnsolicitedAuthnResponse";
        /// <summary>
        /// Are unsolicited responses from the idps in the federation allowed?
        /// </summary>
        [ConfigurationProperty(allowUnsolicitedAuthnResponse)]
        public bool AllowUnsolicitedAuthnResponse
        {
            get
            {
                return (bool)base[allowUnsolicitedAuthnResponse];
            }
        }
    }
}
