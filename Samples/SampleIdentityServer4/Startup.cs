using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Kentor.AuthServices.Configuration;
using Microsoft.IdentityModel.Tokens.Saml2;
using Kentor.AuthServices;
using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.WebSso;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;

namespace SampleIdentityServer4
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(GetIdentityResources())
                .AddInMemoryApiResources(GetApiResources())
                .AddInMemoryClients(GetClients())
                .AddTestUsers(GetUsers());

            CommandFactory.AcsCommandName = "AssertionConsumerService";

            services.AddAuthentication()
                .AddSaml2(opt =>
                {
                    opt.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    opt.SPOptions = new SPOptions
                    {
                        EntityId = new Saml2NameIdentifier("https://SampleIdentityServer4")
                    };

                    var idp = new IdentityProvider(
                        new EntityId("https://stubidp.sustainsys.com/Metadata"),
                        opt.SPOptions)
                    {
                        Binding = Saml2BindingType.HttpRedirect,
                        SingleSignOnServiceUrl = new Uri("https://stubidp.sustainsys.com")
                    };

                    idp.SigningKeys.AddConfiguredKey(new X509Certificate2("Kentor.AuthServices.StubIdp.cer"));

                    opt.IdentityProviders.Add(idp);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            
            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
           
        }

        static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("myapi", "My API")
            };
        }

        static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "js-client",

                    AllowedGrantTypes = GrantTypes.Implicit,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedScopes = { "myapi" }
                }
            };
        }

        static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username ="alice",
                    Password = "password"
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password"
                }
            };
        }

        static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }
    }
}
