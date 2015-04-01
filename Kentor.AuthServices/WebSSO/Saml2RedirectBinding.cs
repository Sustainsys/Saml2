using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;

namespace Kentor.AuthServices.WebSso
{
    class Saml2RedirectBinding : Saml2Binding
    {
        public override CommandResult Bind(string payload, Uri destinationUrl, string messageName)
        {
            if (payload == null)
            {
                throw new ArgumentNullException("payload");
            }
            if (destinationUrl == null)
            {
                throw new ArgumentNullException("destinationUrl");
            }

            var serializedRequest = Serialize(payload);

            var delimeter = destinationUrl.ToString().Contains("?") ? "&" : "?";
            var redirectUri = new Uri(string.Format("{0}{1}{2}{3}", destinationUrl, delimeter, "SAMLRequest=", serializedRequest));

            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = redirectUri
            };
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification="The MemoryStream is not disposed by the DeflateStream - we're using the keep-open flag.")]
        public override string Unbind(HttpRequestData request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var payload = Convert.FromBase64String(request.QueryString["SAMLRequest"].First());
            using (var compressed = new MemoryStream(payload))
            {
                using (var decompressedStream = new DeflateStream(compressed, CompressionMode.Decompress, true))
                {
                    using (var deCompressed = new MemoryStream())
                    {
                        decompressedStream.CopyTo(deCompressed);
                        var xmlData = Encoding.UTF8.GetString(deCompressed.GetBuffer());
                        return xmlData;
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification="The MemoryStream is not disposed by the DeflateStream - we're using the keep-open flag.")]
        private static string Serialize(string payload)
        {
            using (var compressed = new MemoryStream())
            {
                using (var writer = new StreamWriter(new DeflateStream(compressed, CompressionLevel.Optimal, true)))
                {
                    writer.Write(payload);
                }

                return WebUtility.UrlEncode(Convert.ToBase64String(compressed.GetBuffer()));
            }
        }
    }
}
