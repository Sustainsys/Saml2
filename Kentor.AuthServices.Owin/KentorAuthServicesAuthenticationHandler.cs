using Kentor.AuthServices.Configuration;
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
            var result = CommandFactory.GetCommand("acs").Run(await Context.ToHttpRequestData());

            var properties = new AuthenticationProperties()
            {
                RedirectUri = result.Location.ToString()
            };

            var identities = result.Principal.Identities.Select(i =>
                new ClaimsIdentity(i, null, Options.SignInAsAuthenticationType, i.NameClaimType, i.RoleClaimType));

            return new MultipleIdentityAuthenticationTicket(identities, properties);
        }

        protected override Task ApplyResponseChallengeAsync()
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
                    var result = SignInCommand.CreateResult(idp, challenge.Properties.RedirectUri, Context.Request.Uri);
                    Response.Redirect(result.Location.ToString());
                }
            }

            return Task.FromResult(0);
        }

        public override async Task<bool> InvokeAsync()
        {
            if (KentorAuthServicesSection.Current.AssertionConsumerServiceUrl == Request.Uri)
            {
                var ticket = (MultipleIdentityAuthenticationTicket)await AuthenticateAsync();

                Context.Authentication.SignIn(ticket.Properties, ticket.Identities.ToArray());

                Response.Redirect(ticket.Properties.RedirectUri);

                return true;
            }
            
            if(Request.Path == Options.MetadataPath)
            {
                CommandFactory.GetCommand("").Run(await Context.ToHttpRequestData()).Apply(Context);
                return true;
            }

            return false;
        }
    }
}
