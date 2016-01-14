using System;
using System.Globalization;
using System.Linq;

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
    }
}