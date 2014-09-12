using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IO;
using System.Linq;
using System.Web;

namespace Kentor.AuthServices.StubIdp
{
    public static class MetadataBaseExtensions
    {
        public static string ToXmlString(this MetadataBase metadata)
        {
            var serializer = new MetadataSerializer();

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