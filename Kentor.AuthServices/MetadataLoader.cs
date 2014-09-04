using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Helper for loading SAML2 metadata
    /// </summary>
    public static class MetadataLoader
    {
        /// <summary>
        /// Load and parse metadata.
        /// </summary>
        /// <param name="metadataUri">Uri to metadata</param>
        /// <returns>EntityDescriptor containing metadata</returns>
        public static EntityDescriptor Load(Uri metadataUri)
        {
            if (metadataUri == null)
            {
                throw new ArgumentNullException("metadataUri");
            }
            
            using(var client = new WebClient())
            using(var stream = client.OpenRead(metadataUri.ToString()))
            {
                var serializer = new MetadataSerializer();
                return (EntityDescriptor) serializer.ReadMetadata(stream);
            }
        }
    }
}
