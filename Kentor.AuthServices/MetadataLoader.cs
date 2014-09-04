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
        /// <param name="metadataUrl">Url to metadata</param>
        /// <returns>EntityDescriptor containing metadata</returns>
        public static EntityDescriptor LoadIdp(Uri metadataUrl)
        {
            if (metadataUrl == null)
            {
                throw new ArgumentNullException("metadataUrl");
            }

            return (EntityDescriptor)Load(metadataUrl);
        }

        private static MetadataBase Load(Uri metadataUrl)
        {
            using (var client = new WebClient())
            using (var stream = client.OpenRead(metadataUrl.ToString()))
            {
                var serializer = new MetadataSerializer();
                return serializer.ReadMetadata(stream);
            }
        }

        /// <summary>
        /// Load and parse metadata for a federation.
        /// </summary>
        /// <param name="metadataUrl">Url to metadata</param>
        /// <returns></returns>
        public static EntitiesDescriptor LoadFederation(Uri metadataUrl)
        {
            if (metadataUrl == null)
            {
                throw new ArgumentNullException("metadataUrl");
            }

            return (EntitiesDescriptor)Load(metadataUrl);
        }
    }
}
