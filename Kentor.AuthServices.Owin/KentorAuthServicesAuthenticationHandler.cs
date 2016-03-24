using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.WebSso;
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
            var acsPath = new PathString(Options.SPOptions.ModulePath)
                .Add(new PathString("/" + CommandFactory.AcsCommandName));

            if (Request.Path != acsPath)
            {
                return null;
            }

            var result = CommandFactory.GetCommand(CommandFactory.AcsCommandName)
                .Run(await Context.ToHttpRequestData(Options.DataProtector.Unprotect), Options);

            var identities = result.Principal.Identities.Select(i =>
                new ClaimsIdentity(i, null, Options.SignInAsAuthenticationType, i.NameClaimType, i.RoleClaimType));

            var authProperties = (AuthenticationProperties)result.RelayData ?? new AuthenticationProperties();
            authProperties.RedirectUri = result.Location.OriginalString;

            return new MultipleIdentityAuthenticationTicket(identities, authProperties);
        }

        protected override async Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode == 401)
            {
                var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

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
                    var result = SignInCommand.Run(
                        idp,
                        challenge.Properties.RedirectUri,
                        await Context.ToHttpRequestData(Options.DataProtector.Unprotect),
                        Options,
                        challenge.Properties);

                    result.Apply(Context, Options.DataProtector);
                }
            }
        }

        protected async override Task ApplyResponseGrantAsync()
        {
            var revoke = Helper.LookupSignOut(Options.AuthenticationType, Options.AuthenticationMode);

            if (revoke != null)
            {
                var request = await Context.ToHttpRequestData(Options.DataProtector.Unprotect);
                var urls = new AuthServicesUrls(request, Options.SPOptions);

                string redirectUrl;
                if(Context.Response.StatusCode / 100 == 3)
                {
                    var locationUrl = Context.Response.Headers["Location"];

                    redirectUrl = new Uri(
                        new Uri(urls.ApplicationUrl.ToString().TrimEnd('/') + Context.Request.Path),
                        locationUrl
                        ).ToString();
                }
                else
                {
                    redirectUrl = new Uri(
                        urls.ApplicationUrl,
                        Context.Request.Path.ToUriComponent().TrimStart('/'))
                        .ToString();
                }

                LogoutCommand.Run(request, redirectUrl, Options)
                    .Apply(Context, Options.DataProtector);
            }

            await AugmentAuthenticationGrantWithLogoutClaims(Context);
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
                    .Run(await Context.ToHttpRequestData(Options.DataProtector.Unprotect), Options)
                    .Apply(Context, Options.DataProtector);

                return true;
            }

            return false;
        }

        private async Task AugmentAuthenticationGrantWithLogoutClaims(IOwinContext context)
        {
            var grant = context.Authentication.AuthenticationResponseGrant;
            var externalIdentity = await context.Authentication.AuthenticateAsync(Options.SignInAsAuthenticationType);
            var sessionIdClaim = externalIdentity?.Identity.FindFirst(AuthServicesClaimTypes.SessionIndex);
            var externalNameIdClaim = externalIdentity?.Identity.FindFirst(ClaimTypes.NameIdentifier);

            if (grant == null || externalIdentity == null || sessionIdClaim == null || externalNameIdClaim == null)
            {
                return;
            }

            grant.Identity.AddClaim(new Claim(
                sessionIdClaim.Type,
                sessionIdClaim.Value,
                sessionIdClaim.ValueType,
                sessionIdClaim.Issuer));

            var logoutNameIdClaim = new Claim(
                AuthServicesClaimTypes.LogoutNameIdentifier,
                externalNameIdClaim.Value,
                externalNameIdClaim.ValueType,
                externalNameIdClaim.Issuer);

            foreach(var kv in externalNameIdClaim.Properties)
            {
                logoutNameIdClaim.Properties.Add(kv);
            }

            grant.Identity.AddClaim(logoutNameIdClaim);
        }
    }
}
