using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kentor.AuthServices
{
    class SignInCommand : ICommand
    {
        public CommandResult Run(HttpRequestData request)
        {
            if(request == null)
            {
                throw new ArgumentNullException("request");
            }

            return CreateResult(request.QueryString["idp"], request.QueryString["ReturnUrl"], request.Url);
        }

        public static CommandResult CreateResult(string idpName, string returnPath, Uri requestUrl)
        {
            IdentityProvider idp;
            if (!string.IsNullOrEmpty(idpName))
            {
                var selectedIssuer = HttpUtility.UrlDecode(idpName);
                if (!IdentityProvider.ConfiguredIdentityProviders.TryGetValue(selectedIssuer, out idp))
                {
                    throw new InvalidOperationException("Unknown idp");
                }
            }
            else
            {
                idp = IdentityProvider.ConfiguredIdentityProviders.First().Value;
            }

            Uri returnUri = null;
            if (!string.IsNullOrEmpty(returnPath))
            {
                Uri.TryCreate(requestUrl, returnPath, out returnUri);
            }

            var authnRequest = idp.CreateAuthenticateRequest(returnUri);

            return idp.Bind(authnRequest);
        }
    }
}
