using Kentor.AuthServices.Saml2P;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Kentor.AuthServices.WebSso
{
    class Saml2RedirectBinding : Saml2Binding
    {
        public override CommandResult Bind(ISaml2Message message, Uri destinationUrl)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (destinationUrl == null)
            {
                throw new ArgumentNullException("destinationUrl");
            }

            var authnRequest = message as Saml2RequestBase;            

            var redirectUrl = "SAMLRequest=" + Serialize(authnRequest.ToXml());

            if (authnRequest.SigningCertificate != null)
            {
                redirectUrl += "&SigAlg=" + HttpUtility.UrlEncode(System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url);

                // Calculate the signature of the URL as described in [SAMLBind] section 3.4.4.1.
                var signature = SignData((RSACryptoServiceProvider)authnRequest.SigningCertificate.PrivateKey, Encoding.UTF8.GetBytes(redirectUrl));
                var base64Sig = Convert.ToBase64String(signature);

                redirectUrl += "&Signature=" + HttpUtility.UrlEncode(base64Sig);
            }

            var redirectUri = new Uri(destinationUrl + "?" + redirectUrl);

            return new CommandResult
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = redirectUri
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "The MemoryStream is not disposed by the DeflateStream - we're using the keep-open flag.")]
        public override string Unbind(HttpRequestData request)
        {
            if (request == null || request.QueryString["SAMLRequest"] == null)
            {
                return null;
            }

            return Deserialize(request.QueryString["SAMLRequest"]);
        }

        /// <summary>
        /// Create the signature for the data.
        /// </summary>
        /// <param name="signingKey"></param>
        /// <param name="data">The data.</param>
        /// <returns>SignData based on passed data and SigningKey.</returns>
        private static byte[] SignData(RSACryptoServiceProvider signingKey, byte[] data)
        {
            using (var provider = new SHA1CryptoServiceProvider())
            {
                var rsa = signingKey;
                return rsa.SignData(data, provider);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "The MemoryStream is not disposed by the DeflateStream - we're using the keep-open flag.")]
        private static string Deserialize(string serializedRequest)
        {
            var payload = Convert.FromBase64String(serializedRequest);
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification="The MemoryStream is not disposed by the DeflateStream - we're using the keep-open flag.")]
        private static string Serialize(string payload)
        {
            using (var compressed = new MemoryStream())
            {
                using (var writer = new StreamWriter(new DeflateStream(compressed, CompressionLevel.Optimal, true)))
                {
                    writer.Write(payload);
                }

                return HttpUtility.UrlEncode(Convert.ToBase64String(compressed.GetBuffer()));
            }
        }
    }
}
