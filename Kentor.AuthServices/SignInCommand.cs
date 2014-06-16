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

            IdentityProvider idp;
            if (!string.IsNullOrEmpty(request.QueryString["idp"]))
            {
                var selectedIssuer = HttpUtility.UrlDecode(request.QueryString["idp"]);
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
            if (!string.IsNullOrEmpty(request.QueryString["ReturnUrl"]))
            {
                Uri.TryCreate(request.Url, request.QueryString["ReturnUrl"], out returnUri);
            }

            var authnRequest = idp.CreateAuthenticateRequest(returnUri);

            return idp.Bind(authnRequest);
        }
    }
}
