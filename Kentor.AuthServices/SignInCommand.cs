using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kentor.AuthServices
{
    class SignInCommand : ICommand
    {
        public CommandResult Run(HttpRequestData request, IOptions options)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            return CreateResult(
                new EntityId(request.QueryString["idp"]),
                request.QueryString["ReturnUrl"],
                request.Url,
                options.SPOptions);
        }

        public static CommandResult CreateResult(
            EntityId idpEntityId,
            string returnPath,
            Uri requestUrl,
            ISPOptions spOptions)
        {
            IdentityProvider idp;
            if (idpEntityId == null || idpEntityId.Id == null)
            {
                if (spOptions.DiscoveryServiceUrl != null)
                {
                    return RedirectToDiscoveryService(returnPath, spOptions);
                }
                idp = IdentityProvider.ActiveIdentityProviders.First();
            }
            else
            {
                if (!IdentityProvider.ActiveIdentityProviders.TryGetValue(idpEntityId, out idp))
                {
                    throw new InvalidOperationException("Unknown idp");
                }
            }

            Uri returnUri = null;
            if (!string.IsNullOrEmpty(returnPath))
            {
                Uri.TryCreate(requestUrl, returnPath, out returnUri);
            }

            var authnRequest = idp.CreateAuthenticateRequest(returnUri);

            return idp.Bind(authnRequest);
        }

        private static CommandResult RedirectToDiscoveryService(string returnPath, ISPOptions spOptions)
        {
            string returnUrl = spOptions.DiscoveryServiceResponseUrl.OriginalString;

            if(!string.IsNullOrEmpty(returnPath))
            {
                returnUrl += "?ReturnUrl=" + Uri.EscapeDataString(returnPath);
            }

            var redirectLocation = string.Format(
                CultureInfo.InvariantCulture,
                "{0}?entityID={1}&return={2}&returnIDParam=idp",
                spOptions.DiscoveryServiceUrl,
                Uri.EscapeDataString(spOptions.EntityId.Id),
                Uri.EscapeDataString(returnUrl));

            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = new Uri(redirectLocation)
            };
        }
    }
}
