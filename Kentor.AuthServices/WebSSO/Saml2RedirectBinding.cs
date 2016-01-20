using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Saml2P;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
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

            var queryString = message.MessageName + "=" + serializedRequest
                + (string.IsNullOrEmpty(message.RelayState) ? ""
                    : ("&RelayState=" + Uri.EscapeDataString(message.RelayState)));

            if(message.SigningCertificate != null)
            {
                queryString = AddSignature(queryString, message.SigningCertificate);
            }

            var redirectUri = new Uri(message.DestinationUrl.ToString()
                + (String.IsNullOrEmpty(message.DestinationUrl.Query) ? "?" : "&")
                + queryString);

            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = redirectUri
            };
        }

        private static string AddSignature(string queryString, X509Certificate2 key)
        {
            queryString += "&SigAlg=" + Uri.EscapeDataString(SignedXml.XmlDsigRSASHA1Url);

            var signatureDescription = 
                (SignatureDescription)CryptoConfig.CreateFromName(SignedXml.XmlDsigRSASHA1Url);

            var hashAlg = signatureDescription.CreateDigest();
            hashAlg.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            var asymmetricSignatureFormatter = signatureDescription.CreateFormatter(key.PrivateKey);
            var signatureValue = asymmetricSignatureFormatter.CreateSignature(hashAlg);

            queryString += "&Signature=" + Uri.EscapeDataString(Convert.ToBase64String(signatureValue));

            return queryString;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification="The MemoryStream is not disposed by the DeflateStream - we're using the keep-open flag.")]
        public override UnbindResult Unbind(HttpRequestData request, IOptions options)
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

                        var xml = new XmlDocument()
                        {
                            PreserveWhitespace = true
                        };

                        xml.LoadXml(Encoding.UTF8.GetString(deCompressed.GetBuffer()));

                        return new UnbindResult(
                            xml.DocumentElement,
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
