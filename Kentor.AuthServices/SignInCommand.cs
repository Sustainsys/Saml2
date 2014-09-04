using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IdentityModel.Metadata;
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

            return CreateResult(new EntityId(request.QueryString["idp"]), 
                request.QueryString["ReturnUrl"], request.Url);
        }

        public static CommandResult CreateResult(EntityId idpEntityId, string returnPath, Uri requestUrl)
        {
            IdentityProvider idp;
            if (idpEntityId != null && idpEntityId.Id != null)
            {
                if (!IdentityProvider.ActiveIdentityProviders.TryGetValue(idpEntityId, out idp))
                {
                    throw new InvalidOperationException("Unknown idp");
                }
            }
            else
            {
                idp = IdentityProvider.ActiveIdentityProviders.First();
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
