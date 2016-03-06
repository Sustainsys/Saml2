using IdentityServer3.Core.Configuration;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityServer3.Core.Services.InMemory;
using Microsoft.Owin;
using IdentityServer3.Core.Models;
using Kentor.AuthServices.Owin;
using Kentor.AuthServices.Configuration;
using System.IdentityModel.Metadata;
using Kentor.AuthServices;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Owin.Security;
using IdentityServer3.Core.Extensions;

[assembly: OwinStartupAttribute(typeof(SampleIdentityServer3.PostLogoutRedirect.Startup))]

namespace SampleIdentityServer3.PostLogoutRedirect
{
    public class Startup
    {
        public const string domain = "http://localhost:3889/";
        public const string authority = "http://localhost:3889/IdSrv3";

        public void Configuration(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Trace()
                .CreateLogger();

            app.Map("/IdSrv3", idSrv3App =>
            {
                var options = new IdentityServerOptions
                {
                    SiteName = "Embedded IdentityServer",

                    Factory = new IdentityServerServiceFactory()
                        .UseInMemoryScopes(StandardScopes.All)
                        .UseInMemoryClients(Clients.Get())
                        .UseInMemoryUsers(Users.Get()),

                    AuthenticationOptions = new IdentityServer3.Core.Configuration.AuthenticationOptions()
                    {
                        EnablePostSignOutAutoRedirect = true,
                        EnableSignOutPrompt = false,
                    },

                    RequireSsl = false,

                    SigningCertificate = 
                        new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\Kentor.AuthServices.Tests.pfx")
                };

                options.AuthenticationOptions.IdentityProviders = ConfigureSaml2;

                idSrv3App.UseIdentityServer(options);
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = authority,
                ClientId = "OpenidClient",
                RedirectUri = domain,
                ResponseType = "id_token",
                SignInAsAuthenticationType = "Cookies",
                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    RedirectToIdentityProvider = (context) =>
                    {
                        context.ProtocolMessage.RedirectUri = domain;
                        context.ProtocolMessage.PostLogoutRedirectUri = domain;

                        if (context.ProtocolMessage.RequestType == Microsoft.IdentityModel.Protocols.OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = context.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                context.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }

                        return Task.FromResult(0);
                    },
                    // we use this notification for injecting our custom logic
                    SecurityTokenValidated = (context) =>
                    {
                        context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim("id_token", context.ProtocolMessage.IdToken));
                        return Task.FromResult(0);
                    }
                }
            });

            app.Map("/login", loginApp =>
            {
                loginApp.Use((ctx, next) =>
                {
                    ctx.Authentication.Challenge(
                        new AuthenticationProperties
                        {
                            RedirectUri = "/"
                        },
                        OpenIdConnectAuthenticationDefaults.AuthenticationType);
                    
                    return Task.FromResult(0);
                });
            });

            app.Map("/logout", logoutApp =>
            {
                logoutApp.Use((ctx, next) =>
                {
                    ctx.Authentication.SignOut();
                    return Task.FromResult(0);
                });
            });
        }

        private void ConfigureSaml2(IAppBuilder app, string signInAsType)
        {

            var options = new KentorAuthServicesAuthenticationOptions(false)
            {
                SPOptions = new SPOptions
                {
                    EntityId = new EntityId($"{authority}/AuthServices"),
                },
                SignInAsAuthenticationType = signInAsType,
                Caption = "SAML2p"
            };

            UseIdSrv3PostRedirectLogout(app);

            options.SPOptions.ServiceCertificates.Add(new X509Certificate2(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/App_Data/Kentor.AuthServices.Tests.pfx"));

            options.IdentityProviders.Add(new IdentityProvider(
                new EntityId("http://localhost:52071/Metadata"),
                options.SPOptions)
            {
                LoadMetadata = true
            });

            app.UseKentorAuthServicesAuthentication(options);
        }

        private void UseIdSrv3PostRedirectLogout(IAppBuilder app)
        {
            app.Map("/signoutcallback", cleanup =>
            {
                cleanup.Run(async ctx =>
                {
                    var state = ctx.Request.Cookies["state"];
                    await ctx.Environment.RenderLoggedOutViewAsync(state);
                });
            });

            app.Use(async (context, next) =>
            {
                //save idsv logout state if present. this has to be done before headers are written
                var signOutMessageId = context.Environment.GetSignOutMessageId();
                if (signOutMessageId != null)
                    context.Response.Cookies.Append("state", signOutMessageId);

                await next.Invoke();

                //hook into Kentor 303 Redirect to Idsv logout page
                var state = context.Request.Cookies["state"];
                if (state != null && context.Response.StatusCode == 303 && context.Response.Headers["Location"] == $"{authority}/logout")
                {
                    context.Response.Redirect($"{authority}/signoutcallback");
                }
            });
        }
    }
}