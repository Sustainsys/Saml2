using Kentor.AuthServices.Saml2P;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Kentor.AuthServices.WebSso
{
    sealed class Saml2RedirectBinding : Saml2Binding
    {
        public override CommandResult Bind(ISaml2Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var serializedRequest = Serialize(message.ToXml());

            var redirectUri = new Uri(message.DestinationUrl.ToString()
                + (String.IsNullOrEmpty(message.DestinationUrl.Query) ? "?" : "&") 
                + message.MessageName + "=" + serializedRequest
                + (string.IsNullOrEmpty(message.RelayState) ? ""
                    : ("&RelayState=" + Uri.EscapeDataString(message.RelayState))));

            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = redirectUri
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification="The MemoryStream is not disposed by the DeflateStream - we're using the keep-open flag.")]
        public override UnbindResult Unbind(HttpRequestData request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var payload = Convert.FromBase64String(request.QueryString["SAMLRequest"].First());
            using (var compressed = new MemoryStream(payload))
            {
                using (var decompressedStream = new DeflateStream(compressed, CompressionMode.Decompress, true))
                {
                    using (var deCompressed = new MemoryStream())
                    {
                        decompressedStream.CopyTo(deCompressed);

                        return new UnbindResult(
                            Encoding.UTF8.GetString(deCompressed.GetBuffer()),
                            request.QueryString["RelayState"].SingleOrDefault());
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification="The MemoryStream is not disposed by the DeflateStream - we're using the keep-open flag.")]
        private static string Serialize(string payload)
        {
            using (var compressed = new MemoryStream())
            {
                using (var writer = new StreamWriter(new DeflateStream(compressed, CompressionLevel.Optimal, true)))
                {
                    writer.Write(payload);
                }

                return System.Net.WebUtility.UrlEncode(Convert.ToBase64String(compressed.GetBuffer()));
            }
        }
    }
}
