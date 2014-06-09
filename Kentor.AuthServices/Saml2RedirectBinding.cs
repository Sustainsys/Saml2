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

namespace Kentor.AuthServices
{
    class Saml2RedirectBinding : Saml2Binding
    {
        public override CommandResult Bind(ISaml2Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            var serializedRequest = Serialize(message);

            var redirectUri = new Uri(message.DestinationUri.ToString()
                + "?SAMLRequest=" + serializedRequest);

            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = redirectUri
            };
        }

        // The MemoryStream is not disposed by the DeflateStream - we're using the keep-open flag.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public override TSaml2Message Unbind<TSaml2Message>(HttpRequestBase request)
        {
            if (request == null || request["SAMLRequest"] == null)
            {
                return null;
            }
            var payload = Convert.FromBase64String(request["SAMLRequest"]);
            using (var compressed = new MemoryStream(payload))
            {
                using (var decompressedStream = new DeflateStream(compressed, CompressionMode.Decompress, true))
                {
                    using (var deCompressed = new MemoryStream())
                    {
                        decompressedStream.CopyTo(deCompressed);
                        var xmlData = System.Text.Encoding.UTF8.GetString(deCompressed.GetBuffer());
                        if (typeof(TSaml2Message) == typeof(Saml2Response))
                        {
                            return Saml2Response.Read(xmlData) as TSaml2Message;
                        }
                        else if (typeof(TSaml2Message) == typeof(Saml2AuthenticationRequest))
                        {
                            return Saml2AuthenticationRequest.Read(xmlData) as TSaml2Message;
                        }
                        else
                        {
                            throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, "Can't unbind {0}", typeof(TSaml2Message).Name));
                        }
                    }
                }
            }
        }

        // The MemoryStream is not disposed by the DeflateStream - we're using the keep-open flag.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private static string Serialize(ISaml2Message request)
        {
            using (var compressed = new MemoryStream())
            {
                using (var writer = new StreamWriter(new DeflateStream(compressed, CompressionLevel.Optimal, true)))
                {
                    writer.Write(request.ToXml());
                }

                return HttpUtility.UrlEncode(Convert.ToBase64String(compressed.GetBuffer()));
            }
        }
    }
}
