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
        /// <summary>
        /// Url to download metdata for the federation from.
        /// </summary>
        [ConfigurationProperty("metadataUrl", IsRequired=true)]
        public Uri MetadataUrl
        {
            get
            {
                return (Uri)base["metadataUrl"];
            }
        }
    }
}
