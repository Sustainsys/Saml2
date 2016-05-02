using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Exceptions;
using Kentor.AuthServices.Saml2P;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
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
    class Saml2RedirectBinding : Saml2Binding
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
                + (string.IsNullOrEmpty(message.DestinationUrl.Query) ? "?" : "&")
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

            var payload = Convert.FromBase64String(
                request.QueryString["SAMLRequest"].FirstOrDefault()
                ?? request.QueryString["SAMLResponse"].First());
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
                            request.QueryString["RelayState"].SingleOrDefault(),
                            GetTrustLevel(xml.DocumentElement, request, options));
                    }
                }
            }
        }

        private static TrustLevel GetTrustLevel(
            XmlElement documentElement,
            HttpRequestData request,
            IOptions options)
        {
            if(options ==  null)
            {
                return TrustLevel.None;
            }

            if(!request.QueryString["SigAlg"].Any())
            {
                return TrustLevel.None;
            }

            var issuer = documentElement["Issuer", Saml2Namespaces.Saml2Name]?.InnerText;
            
            if(string.IsNullOrEmpty(issuer))
            {
                return TrustLevel.None;
            }

            IdentityProvider idp;
            if(!options.IdentityProviders.TryGetValue(new EntityId(issuer), out idp))
            {
                throw new InvalidSignatureException(
                    string.Format(CultureInfo.InvariantCulture,
                        "Cannot verify signature of message from unknown sender {0}.",
                        issuer));
            }

            CheckSignature(request, idp);

            return TrustLevel.Signature;
        }

        private static void CheckSignature(HttpRequestData request, IdentityProvider idp)
        {
            // Can't use the query string params as found in HttpReqeustData
            // because they are already unescaped and we need the exact format
            // of the original data.
            var rawQueryStringParams = request.Url.Query.TrimStart('?')
                .Split('&')
                .Select(qp => qp.Split('='))
                .ToDictionary(kv => kv[0], kv => kv[1]);

            var msgParam = "";
            string msg;
            if(rawQueryStringParams.TryGetValue("SAMLRequest", out msg))
            {
                msgParam = "SAMLRequest=" + msg;
            }
            else
            {
                msgParam = "SAMLResponse=" + rawQueryStringParams["SAMLResponse"];
            }

            var relayStateParam = "";
            string relayState;
            if (rawQueryStringParams.TryGetValue("RelayState", out relayState))
            {
                relayStateParam = "&RelayState=" + relayState;
            }

            var signedString = string.Format(CultureInfo.InvariantCulture,
                "{0}{1}&SigAlg={2}",
                msgParam,
                relayStateParam,
                rawQueryStringParams["SigAlg"]);

            var sigAlg = request.QueryString["SigAlg"].Single();

            var signatureDescription = (SignatureDescription)CryptoConfig.CreateFromName(sigAlg);

            var hashAlg = signatureDescription.CreateDigest();
            hashAlg.ComputeHash(Encoding.UTF8.GetBytes(signedString));

            var signature = Convert.FromBase64String(request.QueryString["Signature"].Single());

            if(!idp.SigningKeys.Any(
                kic => signatureDescription.CreateDeformatter(
                    ((AsymmetricSecurityKey)kic.CreateKey()).GetAsymmetricAlgorithm(sigAlg, false))
                    .VerifySignature(hashAlg, signature)))
            {
                throw new InvalidSignatureException(string.Format(CultureInfo.InvariantCulture,
                    "Message from {0} failed signature verification",
                    idp.EntityId.Id));
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

        protected internal override bool CanUnbind(HttpRequestData request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return (request.QueryString["SAMLRequest"].Any()
                || request.QueryString["SAMLResponse"].Any());
        }
    }
}
