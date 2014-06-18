using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Owin
{
    /// <summary>
    /// Owin handler that does the actual job when processing Owin requests.
    /// </summary>
    public class KentorAuthServicesAuthenticationHandler : AuthenticationHandler<KentorAuthServicesAuthenticationOptions>
    {
        /// <summary>
        /// Core authentication method.
        /// </summary>
        /// <returns>Task of AuthenticationTicket.</returns>
        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            throw new NotImplementedException();
        }
    }
}
