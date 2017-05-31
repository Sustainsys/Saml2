using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleAspNetCoreApplication.Data;
using SampleAspNetCoreApplication.Models;
using SampleAspNetCoreApplication.Services;
using Kentor.AuthServices.AspNetCore;
using System.IdentityModel.Metadata;
using Kentor.AuthServices;
using System.Security.Cryptography.X509Certificates;
using Kentor.AuthServices.Configuration;
using System.Globalization;
using Kentor.AuthServices.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace SampleAspNetCoreApplication
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if(env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Register custom claims principal factory for kentor auth services to handle logout claims
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, 
                KentorAuthServicesUserClaimsPrincipalFactory<ApplicationUser, IdentityRole>>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<IdentityOptions> identityOptions)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            var options = CreateAuthServicesOptions(identityOptions.Value);
            app.UseKentorAuthServices(options);

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private KentorAuthServicesOptions CreateAuthServicesOptions(IdentityOptions identityOptions)
        {
            var spOptions = CreateSPOptions();
            var authServicesOptions = new KentorAuthServicesOptions
            {
                SPOptions = spOptions,
                SignInAsAuthenticationType = identityOptions.Cookies.ExternalCookieAuthenticationScheme
            };

            var idp = new IdentityProvider(new EntityId("http://stubidp.kentor.se/Metadata"), spOptions)
            {
                AllowUnsolicitedAuthnResponse = true,
                LoadMetadata = true
            };

            authServicesOptions.IdentityProviders.Add(idp);

            return authServicesOptions;
        }

        private SPOptions CreateSPOptions()
        {
            var swedish = CultureInfo.GetCultureInfo("sv-se");

            var organization = new Organization();
            organization.Names.Add(new LocalizedName("Kentor", swedish));
            organization.DisplayNames.Add(new LocalizedName("Kentor IT AB", swedish));
            organization.Urls.Add(new LocalizedUri(new Uri("http://www.kentor.se"), swedish));

            var spOptions = new SPOptions
            {
                EntityId = new EntityId("http://localhost:56327/AuthServices"),
                ReturnUrl = new Uri("http://localhost:56327/Account/ExternalLoginCallback"),
                // NOTE: DiscoveryService implementation in AuthServices has a 
                // bug (GitHub issue #182) that doesn't preserve auth properties.
                // ASP.NET Core needs auth properties to preserve the LoginProvider
                // DiscoveryServiceUrl = new Uri("http://localhost:52071/DiscoveryService"),
                Organization = organization
            };

            var techContact = new ContactPerson
            {
                Type = ContactType.Technical
            };
            techContact.EmailAddresses.Add("authservices@example.com");
            spOptions.Contacts.Add(techContact);

            var supportContact = new ContactPerson
            {
                Type = ContactType.Support
            };
            supportContact.EmailAddresses.Add("support@example.com");
            spOptions.Contacts.Add(supportContact);

            var attributeConsumingService = new AttributeConsumingService("AuthServices")
            {
                IsDefault = true,
            };

            attributeConsumingService.RequestedAttributes.Add(
                new RequestedAttribute("urn:someName")
                {
                    FriendlyName = "Some Name",
                    IsRequired = true,
                    NameFormat = RequestedAttribute.AttributeNameFormatUri
                });

            attributeConsumingService.RequestedAttributes.Add(
                new RequestedAttribute("Minimal"));

            spOptions.AttributeConsumingServices.Add(attributeConsumingService);

            spOptions.ServiceCertificates.Add(new X509Certificate2(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Kentor.AuthServices.Tests.pfx"));

            return spOptions;
        }
    }
}
