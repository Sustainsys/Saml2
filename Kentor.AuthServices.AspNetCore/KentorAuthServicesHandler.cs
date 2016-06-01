using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
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
                return AuthenticateResult.Skip();
            }

            var result = CommandFactory.GetCommand(CommandFactory.AcsCommandName)
                .Run(await Context.ToHttpRequestDataAsync(Options.DataProtector.Unprotect), Options);

            if(!result.HandledResult)
            {
                result.Apply(Context, Options.DataProtector);
            }

            var authProperties = new AuthenticationProperties(result.RelayData);
            authProperties.RedirectUri = result.Location.OriginalString;

            return AuthenticateResult.Success(new AuthenticationTicket(result.Principal, authProperties, Options.SignInAsAuthenticationType));
        }

        protected override async Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            if(context != null)
            {
                var authProps = new AuthenticationProperties(context.Properties);

                EntityId idp = null;
                string strIdp;
                if(context.Properties.TryGetValue("idp", out strIdp))
                {
                    idp = new EntityId(strIdp);
                }
                var redirectUri = authProps.RedirectUri;
                // Don't serialize the RedirectUri twice.
                authProps.RedirectUri = null;

                var result = SignInCommand.Run(
                    idp,
                    redirectUri,
                    await Context.ToHttpRequestDataAsync(Options.DataProtector.Unprotect),
                    Options,
                    authProps.Items);

                if(!result.HandledResult)
                {
                    result.Apply(Context, Options.DataProtector);
                }

                return true;
            }

            return false;
        }

        protected override async Task HandleSignOutAsync(SignOutContext signOutContext)
        {
            if(signOutContext == null)
            {
                return;
            }

            var authProps = new AuthenticationProperties(signOutContext.Properties);
            var request = await Context.ToHttpRequestDataAsync(Options.DataProtector.Unprotect);
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
                result.Apply(Context, Options.DataProtector);
            }
        }

        public override async Task<bool> HandleRequestAsync()
        {
            var authServicesPath = new PathString(Options.SPOptions.ModulePath);
            PathString remainingPath;

            if(Request.Path.StartsWithSegments(authServicesPath, out remainingPath))
            {
                if(remainingPath == new PathString("/" + CommandFactory.AcsCommandName))
                {
                    var authResult = await HandleAuthenticateOnceAsync();
                    if(!authResult.Succeeded)
                    {
                        return false;
                    }

                    await Context.Authentication.SignInAsync(authResult.Ticket.AuthenticationScheme, authResult.Ticket.Principal, authResult.Ticket.Properties);
                    return true;
                }

                var result = CommandFactory.GetCommand(remainingPath.Value)
                    .Run(await Context.ToHttpRequestDataAsync(Options.DataProtector.Unprotect), Options);

                if(!result.HandledResult)
                {
                    result.Apply(Context, Options.DataProtector);
                }

                return true;
            }

            return false;
        }
    }
}
