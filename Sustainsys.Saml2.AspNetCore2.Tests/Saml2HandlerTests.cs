using FluentAssertions;
using Kentor.AuthServices;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.WebSso;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    [TestClass]
    public class Saml2HandlerTests
    {
        class Saml2HandlerTestContext
        {
            public Saml2HandlerTestContext()
            {
                var options = new Saml2Options();
                options.SPOptions.EntityId = new EntityId("http://sp.example.com/saml2");

                options.IdentityProviders.Add(new IdentityProvider(
                    new EntityId("https://idp.example.com"),
                    options.SPOptions)
                {
                    SingleSignOnServiceUrl = new Uri("https://idp.example.com/sso"),
                    Binding = Saml2BindingType.HttpRedirect
                });
                
                Options = new DummyOptionsMonitor(options);

                Subject = new Saml2Handler(Options, LoggerFactory, UrlEncoder, Clock);

                Subject.InitializeAsync(AuthenticationScheme, HttpContext)
                    .Wait();
            }

            public AuthenticationScheme AuthenticationScheme 
                => new AuthenticationScheme("Saml2", "Saml2", typeof(Saml2Handler));

            public IOptionsMonitor<Saml2Options> Options { get; } 

            public ILoggerFactory LoggerFactory 
                => Substitute.For<ILoggerFactory>();

            public UrlEncoder UrlEncoder
                => Substitute.For<UrlEncoder>();

            public ISystemClock Clock
                => Substitute.For<ISystemClock>();

            public Saml2Handler Subject { get; }

            public HttpContext HttpContext { get; } = TestHelpers.CreateHttpContext();
        }

        [TestMethod]
        public async Task Saml2Handler_RedirectsToDefaultIdpOnChallenge()
        {
            var context = new Saml2HandlerTestContext();

            await context.Subject.ChallengeAsync(null);

            var response = context.HttpContext.Response;
            response.StatusCode.Should().Be(303);
            response.Headers["Location"].Single()
                .Should().StartWith("https://idp.example.com/sso?SAMLRequest=");
        }
    }
}
