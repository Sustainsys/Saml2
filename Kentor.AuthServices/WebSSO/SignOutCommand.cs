using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kentor.AuthServices.Configuration;
using System.Net;
using System.IdentityModel.Metadata;
using System.Threading;
using System.Security.Claims;

namespace Kentor.AuthServices.WebSso
{
    class SignOutCommand : ICommand
    {
        public CommandResult Run(HttpRequestData request, IOptions options)
        {
            var idpEntityId = new EntityId(
                ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Issuer);

            var idp = options.IdentityProviders[idpEntityId];

            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = idp.SingleLogoutServiceUrl
            };
        }
    }
}
