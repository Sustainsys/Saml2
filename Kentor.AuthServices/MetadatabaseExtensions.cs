using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IO;
using System.Linq;
using System.Web;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Extensions for Metadatabase.
    /// </summary>
    public static class MetadataBaseExtensions
    {
        /// <summary>
        /// Use a MetadataSerializer to create an XML string out of metadata.
        /// </summary>
        /// <param name="metadata">Metadata to serialize.</param>
        /// <param name="cacheDuration">Cache duration value to include in output.</param>
        /// <returns>string with metadata contents.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static string ToXmlString(this MetadataBase metadata, TimeSpan cacheDuration)
        {
            var serializer = new CacheAwareMetadataSerializer(cacheDuration);

            using (var stream = new MemoryStream())
            {
                serializer.WriteMetadata(stream, metadata);

                using (var reader = new StreamReader(stream))
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    return reader.ReadToEnd();
                }
            }
        }
    }
}