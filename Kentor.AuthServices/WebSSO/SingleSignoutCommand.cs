using System;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Net;
using System.Security.Claims;
using Kentor.AuthServices.Configuration;

namespace Kentor.AuthServices.WebSso
{
    internal class SingleSignoutCommand : ICommand
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
                new EntityId(request.QueryString["idp"].FirstOrDefault()),
                request.QueryString["ReturnUrl"].FirstOrDefault(),
                request,
                options);
        }

        public static CommandResult CreateResult(
            EntityId idpEntityId,
            string returnPath,
            HttpRequestData request,
            IOptions options,
            object relayData = null)
        {
            var urls = new AuthServicesUrls(request, options.SPOptions);

            IdentityProvider idp;
            if (idpEntityId == null || idpEntityId.Id == null)
            {
                if (options.SPOptions.DiscoveryServiceUrl != null)
                {
                    return RedirectToDiscoveryService(returnPath, options.SPOptions, urls);
                }

                idp = options.IdentityProviders.Default;
            }
            else
            {
                if (!options.IdentityProviders.TryGetValue(idpEntityId, out idp))
                {
                    throw new InvalidOperationException("Unknown idp");
                }
            }

            Uri returnUrl = null;
            if (!string.IsNullOrEmpty(returnPath))
            {
                Uri.TryCreate(request.Url, returnPath, out returnUrl);
            }

            string nameIdentifierValue = string.Empty;
            string nameIdentifierFormat = string.Empty;
            if (request.NameIdentifier != null)
            {
                nameIdentifierValue = request.NameIdentifier.Value;
                nameIdentifierFormat = request.NameIdentifier.Properties[ClaimProperties.SamlNameIdentifierFormat];
            }

            var signoutRequest = idp.CreateSignoutRequest(returnUrl, nameIdentifierValue, nameIdentifierFormat, relayData);

            return idp.Bind(signoutRequest);
        }

        private static CommandResult RedirectToDiscoveryService(
            string returnPath,
            ISPOptions spOptions,
            AuthServicesUrls authServicesUrls)
        {
            string returnUrl = authServicesUrls.SignInUrl.OriginalString;

            if (!string.IsNullOrEmpty(returnPath))
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