using Kentor.AuthServices.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Kentor.AuthServices.Metadata
{
    /// <summary>
    /// Helper for loading SAML2 metadata
    /// </summary>
    public static class MetadataLoader
    {
        /// <summary>
        /// Load and parse metadata.
        /// </summary>
        /// <param name="metadataLocation">Path to metadata. A Url, absolute
        /// path or an app relative path (e.g. ~/App_Data/metadata.xml)</param>
        /// <returns>EntityDescriptor containing metadata</returns>
        public static ExtendedEntityDescriptor LoadIdp(string metadataLocation)
        {
            if (metadataLocation == null)
            {
                throw new ArgumentNullException(nameof(metadataLocation));
            }

            return (ExtendedEntityDescriptor)Load(metadataLocation);
        }

        private static MetadataBase Load(string metadataLocation)
        {
            if(PathHelper.IsWebRootRelative(metadataLocation))
            {
                metadataLocation = PathHelper.MapPath(metadataLocation);
            }

            using (var client = new WebClient())
            using (var stream = client.OpenRead(metadataLocation))
            {
                return Load(stream);
            }
        }

        internal static MetadataBase Load(Stream metadataStream)
        {
            var serializer = ExtendedMetadataSerializer.ReaderInstance;
            using (var reader = XmlDictionaryReader.CreateTextReader(metadataStream, XmlDictionaryReaderQuotas.Max))
            {
                // Filter out the signature from the metadata, as the built in MetadataSerializer
                // doesn't handle the XmlDsigNamespaceUrl http://www.w3.org/2000/09/xmldsig# which
                // is allowed (and for SAMLv1 even recommended).
                using (var filter = new FilteringXmlDictionaryReader(SignedXml.XmlDsigNamespaceUrl, "Signature", reader))
                {
                    return serializer.ReadMetadata(filter);
                }
            }
        }

        /// <summary>
        /// Load and parse metadata for a federation.
        /// </summary>
        /// <param name="metadataLocation">Url to metadata</param>
        /// <returns></returns>
        public static ExtendedEntitiesDescriptor LoadFederation(string metadataLocation)
        {
            if (metadataLocation == null)
            {
                throw new ArgumentNullException(nameof(metadataLocation));
            }

            return (ExtendedEntitiesDescriptor)Load(metadataLocation);
        }
    }
}
