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

[assembly: OwinStartupAttribute(typeof(SampleIdentityServer3.Startup))]

namespace SampleIdentityServer3
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/IdSrv3", idSrv3App =>
            {
                var options = new IdentityServerOptions
                {
                    Factory = new IdentityServerServiceFactory()
                        .UseInMemoryScopes(Scopes.Get())
                        .UseInMemoryClients(new List<Client>())
                        .UseInMemoryUsers(new List<InMemoryUser>()),

                    RequireSsl = false,

                };

                options.AuthenticationOptions.IdentityProviders = ConfigureSaml2;

                idSrv3App.UseIdentityServer(options);
            });
        }

        private void ConfigureSaml2(IAppBuilder app, string signInAsType)
        {
            var options = new KentorAuthServicesAuthenticationOptions(false)
            {
                SPOptions = new SPOptions
                {
                    EntityId = new EntityId("http://localhost:4589/IdSrv3/AuthServices"),
                },
                SignInAsAuthenticationType = signInAsType,
                Caption = "SAML2p"
            };

            options.SPOptions.SystemIdentityModelIdentityConfiguration.AudienceRestriction.AudienceMode 
                = AudienceUriMode.Never;

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
    }
}