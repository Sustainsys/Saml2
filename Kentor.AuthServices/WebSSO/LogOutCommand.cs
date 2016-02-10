using System;
using Kentor.AuthServices.Configuration;
using System.IdentityModel.Metadata;
using System.Security.Claims;

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

            var idpEntityId = new EntityId(
                ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Issuer);

            var idp = options.IdentityProviders[idpEntityId];

            var logoutRequest = idp.CreateLogoutRequest();

            var commandResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(logoutRequest);

            return commandResult;
        }
    }
}
