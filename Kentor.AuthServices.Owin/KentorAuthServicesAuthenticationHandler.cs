using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Owin
{
    class KentorAuthServicesAuthenticationHandler : AuthenticationHandler<KentorAuthServicesAuthenticationOptions>
    {
        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            return Task.FromResult<AuthenticationTicket>(null);
        }

        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode == 401)
            {
                var challenge = Helper.LookupChallenge(Constants.DefaultAuthenticationType, AuthenticationMode.Passive);

                if (challenge != null)
                {
                    var result = CommandFactory.GetCommand("signin")
                        .Run(new HttpRequestData("GET", new Uri("http://sp.example.com")));

                    Response.Redirect(result.Location.ToString());
                }
            }

            return Task.FromResult(0);
        }
    }
}
