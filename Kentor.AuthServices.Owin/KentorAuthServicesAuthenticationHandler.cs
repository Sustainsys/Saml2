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

            if (!result.HandledResult)
            {
                result.Apply(Context, Options.DataProtector);
            }

            var identities = result.Principal.Identities.Select(i =>
                new ClaimsIdentity(i, null, Options.SignInAsAuthenticationType, i.NameClaimType, i.RoleClaimType));

            var authProperties = new AuthenticationProperties(result.RelayData);
            authProperties.RedirectUri = result.Location.OriginalString;
            if(result.SessionNotOnOrAfter.HasValue)
            {
                authProperties.AllowRefresh = false;
                authProperties.ExpiresUtc = result.SessionNotOnOrAfter.Value;
            }

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
                    var redirectUri = challenge.Properties.RedirectUri;
                    // Don't serialize the RedirectUri twice.
                    challenge.Properties.RedirectUri = null;

                    var httpRequestData = await Context.ToHttpRequestData(Options.DataProtector.Unprotect);
                    if (httpRequestData.StoredRequestState != null)
                    {
                        foreach (var item in httpRequestData.StoredRequestState.RelayData)
                        {
                            if (!challenge.Properties.Dictionary.ContainsKey(item.Key))
                            {
                                challenge.Properties.Dictionary.Add(item.Key, item.Value);
                            }
                        }
                    }

                    var result = SignInCommand.Run(
                        idp,
                        redirectUri,
                        httpRequestData,
                        Options,
                        challenge.Properties.Dictionary);

                    if (!result.HandledResult)
                    {
                        result.Apply(Context, Options.DataProtector);
                    }
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

                string redirectUrl = revoke.Properties.RedirectUri;
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    if (Context.Response.StatusCode / 100 == 3)
                    {
                        redirectUrl = Context.Response.Headers["Location"];
                    }
                    else
                    {
                        redirectUrl = Context.Request.Path.ToUriComponent();
                    }
                }

                var result = LogoutCommand.Run(request, redirectUrl, Options);

                if (!result.HandledResult)
                {
                    result.Apply(Context, Options.DataProtector);
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
                    Context.Authentication.SignIn(ticket.Properties, ticket.Identities.ToArray());
                    // No need to redirect here. Command result is applied in AuthenticateCoreAsync.
                    return true;
                }

                var result = CommandFactory.GetCommand(remainingPath.Value)
                    .Run(await Context.ToHttpRequestData(Options.DataProtector.Unprotect), Options);

                if (!result.HandledResult)
                {
                    result.Apply(Context, Options.DataProtector);
                }

                return true;
            }

            return false;
        }

        private async Task AugmentAuthenticationGrantWithLogoutClaims(IOwinContext context)
        {
            var grant = context.Authentication.AuthenticationResponseGrant;
            var externalIdentity = await context.Authentication.AuthenticateAsync(Options.SignInAsAuthenticationType);
            var sessionIdClaim = externalIdentity?.Identity.FindFirst(AuthServicesClaimTypes.SessionIndex);
            var externalLogutNameIdClaim = externalIdentity?.Identity.FindFirst(AuthServicesClaimTypes.LogoutNameIdentifier);

            if (grant == null || externalIdentity == null || sessionIdClaim == null || externalLogutNameIdClaim == null)
            {
                return;
            }

            // Need to create new claims because the claim has a back pointer
            // to the identity it belongs to.
            grant.Identity.AddClaim(new Claim(
                sessionIdClaim.Type,
                sessionIdClaim.Value,
                sessionIdClaim.ValueType,
                sessionIdClaim.Issuer));

            grant.Identity.AddClaim(new Claim(
                externalLogutNameIdClaim.Type,
                externalLogutNameIdClaim.Value,
                externalLogutNameIdClaim.ValueType,
                externalLogutNameIdClaim.Issuer));
        }
    }
}
