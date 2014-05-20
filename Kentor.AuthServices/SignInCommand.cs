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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public CommandResult Run(HttpRequestBase request)
        {
            IdentityProvider idp;
            if (request != null && !string.IsNullOrEmpty(request["issuer"]))
            {
                var selectedIssuer = HttpUtility.UrlDecode(request["issuer"]);
                if (!IdentityProvider.ConfiguredIdentityProviders.TryGetValue(selectedIssuer, out idp))
                {
                    throw new InvalidOperationException("Unknown issuer");
                }
            }
            else
            {
                idp = IdentityProvider.ConfiguredIdentityProviders.First().Value;
            }

            Uri returnUri = null;
            if (request != null && request.Url != null && request["ReturnUrl"] != null)
            {
                returnUri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, request["ReturnUrl"]));
            }

            var authnRequest = idp.CreateAuthenticateRequest(returnUri);

            return idp.Bind(authnRequest);
        }
    }
}
