using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.WebSso;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
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

            var httpRequestData = await Context.ToHttpRequestData(Options.DataProtector.Unprotect);
            try
            {
                var result = CommandFactory.GetCommand(CommandFactory.AcsCommandName)
                    .Run(httpRequestData, Options);

                if (!result.HandledResult)
                {
                    result.Apply(Context, Options.DataProtector);
                }

                var identities = result.Principal.Identities.Select(i =>
                    new ClaimsIdentity(i, null, Options.SignInAsAuthenticationType, i.NameClaimType, i.RoleClaimType));

                var authProperties = new AuthenticationProperties(result.RelayData);
                authProperties.RedirectUri = result.Location.OriginalString;
                if (result.SessionNotOnOrAfter.HasValue)
                {
                    authProperties.AllowRefresh = false;
                    authProperties.ExpiresUtc = result.SessionNotOnOrAfter.Value;
                }

                return new MultipleIdentityAuthenticationTicket(identities, authProperties);
            }
            catch (Exception ex)
            {
                return CreateErrorAuthenticationTicket(httpRequestData, ex);
            }
        }

        private AuthenticationTicket CreateErrorAuthenticationTicket(HttpRequestData httpRequestData, Exception ex)
        {
            AuthenticationProperties authProperties = null;
            if (httpRequestData.StoredRequestState != null)
            {
                authProperties = new AuthenticationProperties(
                    httpRequestData.StoredRequestState.RelayData);

                // ReturnUrl is removed from AuthProps dictionary to save space, need to put it back.
                authProperties.RedirectUri = httpRequestData.StoredRequestState.ReturnUrl.OriginalString;
            }
            else
            {
                authProperties = new AuthenticationProperties
                {
                    RedirectUri = Options.SPOptions.ReturnUrl.OriginalString
                };
            }

            // The Google middleware adds this, so let's follow that example.
            authProperties.RedirectUri = WebUtilities.AddQueryString(
                authProperties.RedirectUri, "error", "access_denied");

            string samlResponse = ex.Data.Contains("Saml2Response")
                ? " The received SAML data is\n" + ex.Data["Saml2Response"]
                : "";

            Options.Logger.WriteError("Saml2 Authentication failed." + samlResponse, ex);
            return new MultipleIdentityAuthenticationTicket(
                Enumerable.Empty<ClaimsIdentity>(),
                authProperties);
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

                    if (redirectUri == null && Options.AuthenticationMode == AuthenticationMode.Active)
                    {
                        redirectUri = Context.Request.Uri.ToString();
                    }

                    var result = SignInCommand.Run(
                        idp,
                        redirectUri,
                        await Context.ToHttpRequestData(Options.DataProtector.Unprotect),
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
            // Automatically sign out, even if passive because passive sign in and auto sign out
            // is typically most common scenario. Unless strict compatibility is set.
            var mode = Options.SPOptions.Compatibility.StrictOwinAuthenticationMode ?
                Options.AuthenticationMode : AuthenticationMode.Active;

            var revoke = Helper.LookupSignOut(Options.AuthenticationType, mode);

            if (revoke != null)
            {
                var request = await Context.ToHttpRequestData(Options.DataProtector.Unprotect);
                var urls = new AuthServicesUrls(request, Options);

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

            if (Request.Path.StartsWithSegments(authServicesPath, out remainingPath))
            {
                if (remainingPath == new PathString("/" + CommandFactory.AcsCommandName))
                {
                    var ticket = (MultipleIdentityAuthenticationTicket)await AuthenticateAsync();
                    if (ticket.Identities.Any())
                    {
                        Context.Authentication.SignIn(ticket.Properties, ticket.Identities.ToArray());
                        // No need to redirect here. Command result is applied in AuthenticateCoreAsync.
                    }
                    else
                    {
                        Response.Redirect(ticket.Properties.RedirectUri);
                    }
                    return true;
                }

                try
                {
                    var result = CommandFactory.GetCommand(remainingPath.Value)
                        .Run(await Context.ToHttpRequestData(Options.DataProtector.Unprotect), Options);

                    if (!result.HandledResult)
                    {
                        result.Apply(Context, Options.DataProtector);
                    }

                    return true;
                }
                catch(Exception ex)
                {
                    Options.Logger.WriteError("Error in AuthServices for " + Request.Path, ex);
                    throw;
                }
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
