using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Kentor.AuthServices.AspNetCore
{
    public class KentorAuthServicesHandler : AuthenticationHandler<KentorAuthServicesOptions>
    {
        /// <summary>
        /// Creates a handler instance for use when processing a request.
        /// </summary>
        /// <returns>Handler instance.</returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
