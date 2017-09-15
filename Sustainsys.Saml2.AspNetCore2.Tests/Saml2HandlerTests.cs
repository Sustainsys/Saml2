using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
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
                OptionsMonitor.CurrentValue.Returns(new Saml2Options());
                Subject = new Saml2Handler(OptionsMonitor, LoggerFactory, UrlEncoder, Clock);

                Subject.InitializeAsync(AuthenticationScheme, HttpContext)
                    .Wait();
            }

            public AuthenticationScheme AuthenticationScheme { get; }
                = new AuthenticationScheme("Saml2", "Saml2", typeof(Saml2Handler));

            public IOptionsMonitor<Saml2Options> OptionsMonitor { get; }
                = Substitute.For<IOptionsMonitor<Saml2Options>>();

            public ILoggerFactory LoggerFactory { get; } 
                = Substitute.For<ILoggerFactory>();

            public UrlEncoder UrlEncoder { get; }
                = Substitute.For<UrlEncoder>();

            public ISystemClock Clock { get; }
                = Substitute.For<ISystemClock>();

            public Saml2Handler Subject { get; }

            public HttpContext HttpContext { get; }
                = Substitute.For<HttpContext>();

        }

        [TestMethod]
        public async Task Saml2Handler_RedirectsToDefaultIdpOnChallenge()
        {
            var context = new Saml2HandlerTestContext();

            await context.Subject.ChallengeAsync(null);

            context.HttpContext.Response.StatusCode
                .Should().Be(302);
        }
    }
}
