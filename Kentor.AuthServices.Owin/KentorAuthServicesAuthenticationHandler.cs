using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.WebSSO;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Owin
{
    class KentorAuthServicesAuthenticationHandler : AuthenticationHandler<KentorAuthServicesAuthenticationOptions>
    {
        protected async override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var result = CommandFactory.GetCommand(CommandFactory.AcsCommandName)
                .Run(await Context.ToHttpRequestData(), Options);

            var properties = new AuthenticationProperties()
            {
                RedirectUri = result.Location.ToString()
            };

            var identities = result.Principal.Identities.Select(i =>
                new ClaimsIdentity(i, null, Options.SignInAsAuthenticationType, i.NameClaimType, i.RoleClaimType));

            return new MultipleIdentityAuthenticationTicket(identities, properties);
        }

        protected override async Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode == 401)
            {
                var challenge = Helper.LookupChallenge(Options.AuthenticationType, AuthenticationMode.Passive);

                if (challenge != null)
                {
                    EntityId idp;
                    string strIdp;
                    if (challenge.Properties.Dictionary.TryGetValue("idp", out strIdp))
                    {
                        idp = new EntityId(strIdp);
                    }
                    else
                    {
                        object objIdp = null;
                        Context.Environment.TryGetValue("KentorAuthServices.idp", out objIdp);
                        idp = objIdp as EntityId;
                    }
                    var result = SignInCommand.CreateResult(
                        idp,
                        challenge.Properties.RedirectUri,
                        await Context.ToHttpRequestData(),
                        Options);

                    result.Apply(Context);
                }
            }
        }

        public override async Task<bool> InvokeAsync()
        {
            var authServicesPath = new PathString(Options.SPOptions.ModulePath);
            PathString remainingPath;

            if(Request.Path.StartsWithSegments(authServicesPath, out remainingPath))
            {
                if(remainingPath == new PathString("/" + CommandFactory.AcsCommandName))
                {
                    var ticket = (MultipleIdentityAuthenticationTicket)await AuthenticateAsync();
                    Context.Authentication.SignIn(ticket.Properties, ticket.Identities.ToArray());
                    Response.Redirect(ticket.Properties.RedirectUri);
                    return true;
                }

                CommandFactory.GetCommand(remainingPath.Value)
                    .Run(await Context.ToHttpRequestData(), Options)
                    .Apply(Context);

                return true;
            }

            return false;
        }
    }
}
