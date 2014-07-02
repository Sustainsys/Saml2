using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Kentor.AuthServices.TestHelpers
{
    public static class AuthnRequestHelper
    {
        /// <summary>
        /// Extract the requestId from a uri with a query string containing an AuthnRequest.
        /// </summary>
        /// <param name="uri">Uri to extract data from.</param>
        /// <returns></returns>
        public static string GetRequestId(Uri uri)
        {
            var tmp = Convert.FromBase64String(HttpUtility.UrlDecode(uri.Query.Replace("?SAMLRequest=", "")));
            using (var compressed = new MemoryStream(tmp))
            {
                compressed.Seek(0, SeekOrigin.Begin);
                using (var decompressedStream = new DeflateStream(compressed, CompressionMode.Decompress))
                {
                    using (var deCompressed = new MemoryStream())
                    {
                        decompressedStream.CopyTo(deCompressed);
                        var xmlData = System.Text.Encoding.UTF8.GetString(deCompressed.GetBuffer());
                        var requestXml = XDocument.Parse(xmlData);
                        return requestXml.Document.Root.Attribute(XName.Get("ID")).Value;
                    }
                }
            }

        }
    }
}
