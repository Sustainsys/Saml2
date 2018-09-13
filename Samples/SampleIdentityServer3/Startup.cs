using IdentityServer3.Core.Configuration;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityServer3.Core.Services.InMemory;
using Microsoft.Owin;
using IdentityServer3.Core.Models;
using Sustainsys.Saml2.Owin;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Metadata;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Owin.Security;
using IdentityServer3.Core.Extensions;

[assembly: OwinStartupAttribute(typeof(SampleIdentityServer3.Startup))]

namespace SampleIdentityServer3
{
    public class Startup
    {
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

                    RequireSsl = false,

                    AuthenticationOptions = new IdentityServer3.Core.Configuration.AuthenticationOptions
                    {
                        EnableAutoCallbackForFederatedSignout = true,
                        EnableSignOutPrompt = false
                    },

                    SigningCertificate =
                        new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\Sustainsys.Saml2.SampleIdentityServer3.pfx"),

                    LoggingOptions = new LoggingOptions
                    {
                        EnableKatanaLogging = true
                    }
                };

                options.AuthenticationOptions.IdentityProviders = ConfigureSaml2;

                idSrv3App.UseIdentityServer(options);
            });

            app.Use(async (ctx, next) =>
            {
                await next.Invoke();
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.Use(async (ctx, next) =>
            {
                await next.Invoke();
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = "http://localhost:4589/IdSrv3",
                ClientId = "serverside",
                ClientSecret = "somesecret",
                RedirectUri = "http://localhost:4589/",
                ResponseType = "code id_token",

                SignInAsAuthenticationType = "Cookies"
            });

            app.Use(async (ctx, next) =>
            {
                await next.Invoke();
            });

            app.Map("/ServerSide-Login", loginApp =>
            {
                loginApp.Use((ctx, next) =>
                {
                    ctx.Authentication.Challenge(
                        new AuthenticationProperties
                        {
                            RedirectUri = "http://localhost:4589/"
                        },
                        OpenIdConnectAuthenticationDefaults.AuthenticationType);
                    ctx.Response.StatusCode = 401;

                    return Task.FromResult(0);
                });
            });

            app.Map("/ServerSide-Logout", logoutApp =>
            {
                logoutApp.Use((ctx, next) =>
                {
                    ctx.Authentication.SignOut();
                    ctx.Response.Redirect("/");

                    return Task.FromResult(0);
                });
            });
        }

        private void ConfigureSaml2(IAppBuilder app, string signInAsType)
        {

            var options = new Saml2AuthenticationOptions(false)
            {
                SPOptions = new SPOptions
                {
                    EntityId = new EntityId("http://localhost:4589/IdSrv3/Saml2"),
                },
                SignInAsAuthenticationType = signInAsType,
                Caption = "SAML2p"
            };

            UseIdSrv3LogoutOnFederatedLogout(app, options);

            options.SPOptions.ServiceCertificates.Add(new X509Certificate2(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/App_Data/Sustainsys.Saml2.Tests.pfx"));

            options.IdentityProviders.Add(new IdentityProvider(
                new EntityId("http://localhost:52071/Metadata"),
                options.SPOptions)
            {
                LoadMetadata = true
            });

            app.UseSaml2Authentication(options);
        }

        private void UseIdSrv3LogoutOnFederatedLogout(IAppBuilder app, Saml2AuthenticationOptions options)
        {
            app.Map("/signoutcleanup", cleanup =>
            {
                cleanup.Run(async ctx =>
                {
                    await ctx.Environment.ProcessFederatedSignoutAsync();
                });
            });

            app.Use(async (context, next) =>
            {
                await next.Invoke();

                if (context.Authentication.AuthenticationResponseRevoke != null
                    && context.Response.StatusCode % 100 == 3
                    && !HttpContext.Current.Response.HeadersWritten)
                {
                    var finalLocation = context.Response.Headers["Location"];

                    context.Response.StatusCode = 200;

                    await context.Response.WriteAsync($@"
<html>
    <body>
        <h1>Signing Out...<span id=""dots""></span></h1>
        <iframe style=""display:none;"" src=""../signoutcleanup""></iframe>
        <script>
            setInterval(function() {{ var dots = document.getElementById(""dots""); dots.innerText = dots.innerText + "".""; }}, 250);
            setTimeout(function() {{ window.location = ""{finalLocation}""; }}, 5000);
        </script>
    </body>
</html>");
                }
            });
        }
    }
}