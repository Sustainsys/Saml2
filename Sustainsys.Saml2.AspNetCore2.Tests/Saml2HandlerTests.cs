using FluentAssertions;
using Kentor.AuthServices;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.TestHelpers;
using Kentor.AuthServices.WebSso;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
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

                var idp = new IdentityProvider(
                    new EntityId("https://idp.example.com"),
                    options.SPOptions)
                {
                    SingleSignOnServiceUrl = new Uri("https://idp.example.com/sso"),
                    Binding = Saml2BindingType.HttpRedirect
                };

                idp.SigningKeys.AddConfiguredKey(SignedXmlHelper.TestCert);

                options.IdentityProviders.Add(idp);

                Options = new DummyOptionsMonitor(options);

                Subject = new Saml2Handler(
                    Options, LoggerFactory, UrlEncoder, Clock, new StubDataProtector());

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

            var authProps = new AuthenticationProperties()
            {
                IssuedUtc = new DateTimeOffset(new DateTime(2017, 09, 30)),
                RedirectUri = "https://sp.example.com/LoggedIn"
            };

            var response = context.HttpContext.Response;

            string cookieData = null;
            response.Cookies.Append(
                Arg.Any<string>(),
                Arg.Do<string>(v => cookieData = v),
                Arg.Any<CookieOptions>());

            await context.Subject.ChallengeAsync(authProps);

            response.StatusCode.Should().Be(303);
            response.Headers["Location"].Single()
                .Should().StartWith("https://idp.example.com/sso?SAMLRequest=");

            var state = new StoredRequestState(StubDataProtector.Unprotect(
                HttpRequestData.GetBinaryData(cookieData)));

            state.ReturnUrl.OriginalString.Should().Be("https://sp.example.com/LoggedIn");

            // Don't dual-store the return-url.
            state.RelayData.Values.Should().NotContain("https://sp.example.com/LoggedIn");
        }

        [TestMethod]
        public async Task Saml2Handler_Acs_Works()
        {
            var context = new Saml2HandlerTestContext();

            context.HttpContext.Request.Method = "POST";
            context.HttpContext.Request.Path = "/Saml2/Acs";

            var authProps = new AuthenticationProperties()
            {
                IssuedUtc = new DateTimeOffset(DateTime.UtcNow)
            };

            authProps.Items["Test"] = "TestValue";

            var state = new StoredRequestState(
                new EntityId("https://idp.example.com"),
                new Uri("https://localhost/LoggedIn"),
                new Saml2Id("InResponseToId"),
                authProps.Items);

            var relayState = SecureKeyGenerator.CreateRelayState();

            var cookieData = HttpRequestData.ConvertBinaryData(
                StubDataProtector.Protect(state.Serialize()));

            var cookieName = $"Kentor.{relayState}";

            context.HttpContext.Request.Cookies = new StubCookieCollection(
                Enumerable.Repeat(new KeyValuePair<string, string>(
                    cookieName, cookieData), 1));

            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0""
                IssueInstant=""2013-01-01T00:00:00Z"" InResponseTo=""InResponseToId"" >
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"">
                        <saml2:AudienceRestriction>
                            <saml2:Audience>http://sp.example.com/saml2</saml2:Audience>
                        </saml2:AudienceRestriction>
                    </saml2:Conditions>
                </saml2:Assertion>
            </saml2p:Response>";

            var form = Substitute.For<IFormCollection>();

            IEnumerator<KeyValuePair<string, StringValues>> formCollectionEnumerator =
                new KeyValuePair<string, StringValues>[]
                {
                    new KeyValuePair<string, StringValues>(
                        "SAMLResponse", new StringValues(
                            Convert.ToBase64String(
                                Encoding.UTF8.GetBytes(SignedXmlHelper.SignXml(response))))),
                    new KeyValuePair<string, StringValues>(
                        "RelayState", new StringValues(relayState))
                }.AsEnumerable().GetEnumerator();

            form.GetEnumerator().Returns(formCollectionEnumerator);

            context.HttpContext.Request.Form.Returns(form);

            var authService = Substitute.For<IAuthenticationService>();

            context.HttpContext.RequestServices.GetService(typeof(IAuthenticationService))
                .Returns(authService);

            ClaimsPrincipal principal = null;
            AuthenticationProperties actualAuthProps = null;

            await authService.SignInAsync(
                context.HttpContext,
                "TestSignInScheme",
                Arg.Do<ClaimsPrincipal>(p => principal = p),
                Arg.Do<AuthenticationProperties>(ap => actualAuthProps = ap));

            await context.Subject.HandleRequestAsync();

            principal.HasClaim(ClaimTypes.NameIdentifier, "SomeUser").Should().BeTrue();
            actualAuthProps.IssuedUtc.Should().Be(authProps.IssuedUtc);
            actualAuthProps.Items["Test"].Should().Be("TestValue");

            context.HttpContext.Response.Received().Redirect(state.ReturnUrl.OriginalString);
        }

        [TestMethod]
        public void Saml2Handler_ShouldHandleRequestAsync_ChecksModulePath()
        {
            var context = new Saml2HandlerTestContext();
            context.HttpContext.Request.Path = "/TestPath/Acs";

            context.Options.CurrentValue.SPOptions.ModulePath = "/TestPath";

            context.Subject.ShouldHandleRequestAsync().Result.Should().BeTrue();
        }

        [TestMethod]
        public void Saml2Handler_ShouldHandleRequestAsync_IgnoresCallbackPath()
        {
            var context = new Saml2HandlerTestContext();
            context.HttpContext.Request.Path = "/TestPath";

            context.Options.CurrentValue.CallbackPath = "/TestPath";

            context.Subject.ShouldHandleRequestAsync().Result.Should().BeFalse();
        }
    }
}
