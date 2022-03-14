using FluentAssertions;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.WebSso;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    [TestClass]
    public class CommandResultExtensionsTests
    {

        StoredRequestState GetRequestState() =>
            new StoredRequestState(
                new EntityId("https://idp.example.com"),
                new Uri("https://sp.example.com/ReturnUrl"),
                new Saml2Id(),
                new Dictionary<string, string>()
                {
                                { "Key1", "Value1" },
                                { "Key2", "value2" }
                });

        [TestMethod]
        public async Task CommandResultExtensions_Apply()
        {
            var context = TestHelpers.CreateHttpContext();

            var redirectLocation = "https://destination.com/?q=http%3A%2F%2Fexample.com";
            var commandResult = new CommandResult()
            {
                HttpStatusCode = System.Net.HttpStatusCode.Redirect,
                Location = new Uri(redirectLocation),
                SetCookieName = "Saml2.123",
                SetCookieSecureFlag = true,
                RelayState = "123",
                RequestState = GetRequestState(),
                ContentType = "application/json",
                Content = "{ value: 42 }",
                ClearCookieName = "Clear-Cookie",
                Principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "SomerUser")
                }, "authType")),
                RelayData = new Dictionary<string, string>()
                {
                    { "Relayed", "Value" }
                },
            };

            commandResult.Headers.Add("header", "value");

            ClaimsPrincipal principal = null;
            AuthenticationProperties authProps = null;
            var cookieManager = Substitute.For<ICookieManager>();
            var authService = Substitute.For<IAuthenticationService>();
            context.RequestServices.GetService(typeof(IAuthenticationService))
                .Returns(authService);
            await authService.SignInAsync(
                context,
                "TestSignInScheme",
                Arg.Do<ClaimsPrincipal>(p => principal = p),
                Arg.Do<AuthenticationProperties>(ap => authProps = ap));

            await commandResult.Apply(context, new StubDataProtector(), cookieManager, "TestSignInScheme", null, true);

            var expectedCookieData = HttpRequestData.ConvertBinaryData(
                StubDataProtector.Protect(commandResult.GetSerializedRequestState()));

            context.Response.StatusCode.Should().Be(302);
            context.Response.Headers["Location"].SingleOrDefault()
                .Should().Be(redirectLocation, "location header should be set");

            cookieManager.Received().AppendResponseCookie(
                context,
                "Saml2.123",
                expectedCookieData,
                Arg.Is<CookieOptions>(co => co.HttpOnly && co.Secure && co.SameSite == SameSiteMode.None)
            );

            cookieManager.Received().DeleteCookie(
                context,
                "Clear-Cookie",
                Arg.Is<CookieOptions>(co => co.Secure)
            );

            context.Response.Headers.Received().Add("header", "value");

            context.Response.ContentType
                .Should().Be("application/json", "content type should be set");

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            new StreamReader(context.Response.Body).ReadToEnd()
                .Should().Be("{ value: 42 }", "content should be set");

            principal.HasClaim(ClaimTypes.NameIdentifier, "SomerUser").Should().BeTrue();
            authProps.Items["Relayed"].Should().Be("Value");
            authProps.RedirectUri.Should().Be(redirectLocation);
        }

        [TestMethod]
        public async Task CommandResultExtensions_Apply_NonSecureCookie_NoSameSite()
        {
            var context = TestHelpers.CreateHttpContext();

            var commandResult = new CommandResult
            {
                SetCookieName = "CookieName",
                RequestState = GetRequestState(),
                ClearCookieName = "DeleteName"
            };

            var cookieManager = Substitute.For<ICookieManager>();
            await commandResult.Apply(context, new StubDataProtector(), cookieManager, null, null, false);

            cookieManager.Received().DeleteCookie(
                context,
                "DeleteName",
                Arg.Is<CookieOptions>( co => !co.Secure )
            );

            cookieManager.Received().AppendResponseCookie(
                context,
                "CookieName",
                Arg.Any<string>(),
                Arg.Is<CookieOptions>( co => !co.Secure && co.SameSite == (SameSiteMode)( -1 ) )
            );
        }

        [TestMethod]
        public async Task CommandResultExtensions_Apply_Minimal()
        {
            var context = TestHelpers.CreateHttpContext();

            var commandResult = new CommandResult();

            var authService = Substitute.For<IAuthenticationService>();
            context.RequestServices.GetService(typeof(IAuthenticationService))
                .Returns(authService);

            var cookieManager = Substitute.For<ICookieManager>();
            await commandResult.Apply(context, new StubDataProtector(), cookieManager, "TestSignInScheme", null, false);

            context.Response.StatusCode.Should().Be(200);
            context.Response.Headers.Keys.Should().NotContain("Location");
            cookieManager.DidNotReceive().AppendResponseCookie(
                context, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CookieOptions>());

            await authService.DidNotReceive().SignInAsync(
                Arg.Any<HttpContext>(), Arg.Any<string>(), Arg.Any<ClaimsPrincipal>(), Arg.Any<AuthenticationProperties>());

        }
    }
}
