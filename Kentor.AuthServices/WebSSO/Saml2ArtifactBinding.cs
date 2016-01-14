using System;
using System.Globalization;
using System.Linq;
using Kentor.AuthServices.Saml2P;
using System.Security.Cryptography;
using System.Text;

namespace Kentor.AuthServices.WebSso
{
    internal class Saml2ArtifactBinding : Saml2Binding
    {
        protected internal override bool CanUnbind(HttpRequestData request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return (request.HttpMethod == "GET" && request.QueryString.Contains("SAMLart"))
                || (request.HttpMethod == "POST" && request.Form.ContainsKey("SAMLart"));
        }

        public override UnbindResult Unbind(HttpRequestData request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            string relayState;

            switch(request.HttpMethod)
            {
                case "GET":
                    relayState = request.QueryString["RelayState"].SingleOrDefault();
                    break;
                case "POST":
                    relayState = request.Form["RelayState"];
                    break;
                default:
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                        "Artifact binding can only use GET or POST http method, but found {0}",
                        request.HttpMethod));
            }

            return new UnbindResult(null, relayState);
        }

        private SHA1 sha1 = SHA1.Create();

        public override CommandResult Bind(ISaml2Message message)
        {
            if(message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var artifact = new byte[44];
            artifact[1] = 4; // Header is 0004

            Array.Copy(sha1.ComputeHash(Encoding.UTF8.GetBytes(message.Issuer.Id)),
                0, artifact, 4, 20);

            Array.Copy(SecureKeyGenerator.CreateArtifactMessageHandle(), 0, artifact, 24, 20);

            return new CommandResult
            {
                HttpStatusCode = System.Net.HttpStatusCode.SeeOther,
                Location = new Uri(message.DestinationUrl.OriginalString
                + (string.IsNullOrEmpty(message.DestinationUrl.Query) ? "?" : "&")
                + "SAMLart=" + Uri.EscapeDataString(Convert.ToBase64String(artifact))
                + "&RelayState=" + Uri.EscapeDataString(message.RelayState))
            };
        }
    }
}