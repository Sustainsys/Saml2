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
        public CommandResult Run(HttpRequestData request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return CreateResult(new EntityId(request.QueryString["idp"]),
                request.QueryString["ReturnUrl"], request.Url);
        }

        public static CommandResult CreateResult(EntityId idpEntityId, string returnPath, Uri requestUrl)
        {
            IdentityProvider idp;
            if (idpEntityId == null || idpEntityId.Id == null)
            {
                if (KentorAuthServicesSection.Current.DiscoveryServiceUrl != null)
                {
                    return RedirectToDiscoveryService();
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

        private static CommandResult RedirectToDiscoveryService()
        {
            var redirectLocation = string.Format(
                CultureInfo.InvariantCulture,
                "{0}?entityID={1}&return={2}&returnIDParam=idp",
                KentorAuthServicesSection.Current.DiscoveryServiceUrl,
                Uri.EscapeDataString(KentorAuthServicesSection.Current.EntityId),
                Uri.EscapeDataString(KentorAuthServicesSection.Current.DiscoveryServiceResponseUrl.OriginalString));

            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = new Uri(redirectLocation)
            };
        }
    }
}
