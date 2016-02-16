using System;
using Kentor.AuthServices.Configuration;
using System.IdentityModel.Metadata;
using System.Security.Claims;
using System.Net;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.Exceptions;
using System.Globalization;
using System.Configuration;

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
                var unbindResult = binding.Unbind(request, options);
                switch(unbindResult.Data.LocalName)
                {
                    case "LogoutRequest":
                        return HandleRequest(unbindResult, options);
                    case "LogoutResponse":
                        return HandleResponse(unbindResult, request, options);
                    default:
                        throw new NotImplementedException();
                }
            }
            
            var idpEntityId = new EntityId(
                ClaimsPrincipal.Current.FindFirst(AuthServicesClaimTypes.LogoutNameIdentifier)?.Issuer
                ?? ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Issuer);

            var idp = options.IdentityProviders[idpEntityId];

            var logoutRequest = idp.CreateLogoutRequest();

            var commandResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(logoutRequest);
            commandResult.TerminateLocalSession = true;

            return commandResult;
        }

        private static CommandResult HandleRequest(UnbindResult unbindResult, IOptions options)
        {
            var request = Saml2LogoutRequest.FromXml(unbindResult.Data);

            var idp = options.IdentityProviders[request.Issuer];

            if(options.SPOptions.SigningServiceCertificate == null)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture,
                    "Received a Single Logout request from \"{0}\" but cannot reply because single logout responses must be signed and there is no signing certificate configured. Looks like the idp is configured for Single Logout despite AuthServices not exposing that functionality in the metadata.",
                    request.Issuer.Id));
            }

            var response = new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = idp.SingleLogoutServiceResponseUrl,
                SigningCertificate = options.SPOptions.SigningServiceCertificate,
                InResponseTo = request.Id,
                Issuer = options.SPOptions.EntityId,
            };

            var result = Saml2Binding.Get(idp.SingleLogoutServiceBinding).Bind(response);
            result.TerminateLocalSession = true;
            return result;
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
