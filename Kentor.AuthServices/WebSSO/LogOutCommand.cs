using System;
using Kentor.AuthServices.Configuration;
using System.IdentityModel.Metadata;
using System.Security.Claims;
using System.Net;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.Exceptions;
using System.Globalization;

namespace Kentor.AuthServices.WebSso
{
    class LogoutCommand : ICommand
    {
        public CommandResult Run(HttpRequestData request, IOptions options)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var binding = Saml2Binding.Get(request);
            if(binding != null)
            {
                return HandleResponse(binding.Unbind(request, options), request, options);
            }
            
            var idpEntityId = new EntityId(
                ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Issuer);

            var idp = options.IdentityProviders[idpEntityId];

            var logoutRequest = idp.CreateLogoutRequest();

            var commandResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(logoutRequest);

            return commandResult;
        }

        private static CommandResult HandleResponse(UnbindResult unbindResult, HttpRequestData request, IOptions options)
        {
            var status = Saml2LogoutResponse.FromXml(unbindResult.Data).Status;
            if(status != Saml2StatusCode.Success)
            {
                throw new UnsuccessfulSamlOperationException(string.Format(CultureInfo.InvariantCulture,
                    "Idp returned status \"{0}\", indicating that the single logout failed. The local session has been successfully terminated.",
                    status));
            }

            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = new AuthServicesUrls(request, options.SPOptions).ApplicationUrl
            };
        }
    }
}
