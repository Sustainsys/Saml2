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
        private const string metadataLocation = nameof(metadataLocation);
        /// <summary>
        /// Location (url, local path or app relative path such as ~/App_Data)
        /// where metadata is located.
        /// </summary>
        [ConfigurationProperty(metadataLocation, IsRequired = true)]
        public string MetadataLocation
        {
            get
            {
                return (string)base[metadataLocation];
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
