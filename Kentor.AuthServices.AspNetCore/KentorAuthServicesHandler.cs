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
using Kentor.AuthServices.WebSso;
using System.IdentityModel.Metadata;

namespace Kentor.AuthServices.AspNetCore
{
    public class KentorAuthServicesHandler : AuthenticationHandler<KentorAuthServicesOptions>
    {
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var acsPath = new PathString(Options.SPOptions.ModulePath)
                .Add(new PathString("/" + CommandFactory.AcsCommandName));

            if(Request.Path != acsPath)
            {
                return AuthenticateResult.Fail("acsPath.");
            }

            var result = CommandFactory.GetCommand(CommandFactory.AcsCommandName)
                .Run(await Context.ToHttpRequestData(Options.DataProtector.Unprotect), Options);

            if(!result.HandledResult)
            {
                await result.ApplyAsync(Context, Options.DataProtector);
            }

            var identities = result.Principal.Identities.Select(i =>
                new ClaimsIdentity(i, null, Options.SignInAsAuthenticationType, i.NameClaimType, i.RoleClaimType));

            var authProperties = new AuthenticationProperties(result.RelayData);
            authProperties.RedirectUri = result.Location.OriginalString;

            return AuthenticateResult.Success(new MultipleIdentityAuthenticationTicket(result.Principal, authProperties, Options));
        }

        protected override async Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            if(context != null)
            {
                var authProps = new AuthenticationProperties(context.Properties);

                EntityId idp;
                string strIdp;
                if(context.Properties.TryGetValue("idp", out strIdp))
                {
                    idp = new EntityId(strIdp);
                }
                else
                {
                    object objIdp = null;
                    // TODO: environment?
                    //Context.Environment.TryGetValue("KentorAuthServices.idp", out objIdp);
                    idp = objIdp as EntityId;
                }
                var redirectUri = authProps.RedirectUri;
                // Don't serialize the RedirectUri twice.
                authProps.RedirectUri = null;

                var result = SignInCommand.Run(
                    idp,
                    redirectUri,
                    await Context.ToHttpRequestData(Options.DataProtector.Unprotect),
                    Options,
                    authProps.Items);

                if(!result.HandledResult)
                {
                    await result.ApplyAsync(Context, Options.DataProtector);
                }

                return true;
            }

            return false;
        }

        protected override async Task HandleSignOutAsync(SignOutContext signOutContext)
        {
            if(signOutContext != null)
            {
                var authProps = new AuthenticationProperties(signOutContext.Properties);
                var request = await Context.ToHttpRequestData(Options.DataProtector.Unprotect);
                var urls = new AuthServicesUrls(request, Options.SPOptions);

                string redirectUrl = authProps.RedirectUri;
                if(string.IsNullOrEmpty(redirectUrl))
                {
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
                }

                var result = LogoutCommand.Run(request, redirectUrl, Options);

                if(!result.HandledResult)
                {
                    await result.ApplyAsync(Context, Options.DataProtector);
                }
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
                    Context.Authentication.SignInAsync(ticket.Properties, ticket.Identities.ToArray());
                    // No need to redirect here. Command result is applied in AuthenticateCoreAsync.
                    return true;
                }

                var result = CommandFactory.GetCommand(remainingPath.Value)
                    .Run(await Context.ToHttpRequestData(Options.DataProtector.Unprotect), Options);

                if(!result.HandledResult)
                {
                    await result.ApplyAsync(Context, Options.DataProtector);
                }

                return true;
            }

            return false;
        }

        private async Task AugmentAuthenticationGrantWithLogoutClaims(HttpContext context)
        {
            var grant = context.Authentication.AuthenticationResponseGrant;
            var externalIdentity = await context.Authentication.AuthenticateAsync(Options.SignInAsAuthenticationType);
            var sessionIdClaim = externalIdentity?.FindFirst(AuthServicesClaimTypes.SessionIndex);
            var externalNameIdClaim = externalIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if(grant == null || externalIdentity == null || sessionIdClaim == null || externalNameIdClaim == null)
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
