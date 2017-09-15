using FluentAssertions;
using Kentor.AuthServices;
using Kentor.AuthServices.WebSso;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    [TestClass]
    public class CommandResultExtensionsTests
    {
        [TestMethod]
        public void CommandResultExtensions_Apply()
        {
            var context = TestHelpers.CreateHttpContext();

            var state = new StoredRequestState(
                new EntityId("https://idp.example.com"),
                new Uri("https://sp.example.com/ReturnUrl"),
                new Saml2Id(),
                new Dictionary<string, string>()
                {
                    { "Key1", "Value1" },
                    { "Key2", "value2" }
                });

            var commandResult = new CommandResult()
            {
                HttpStatusCode = System.Net.HttpStatusCode.Redirect,
                Location = new Uri("https://destination.com"),
                SetCookieName = "Saml2.123",
                RelayState = "123",
                RequestState = state,
                ContentType = "application/json",
                Content = "{ value: 42 }",
                ClearCookieName = "Clear-Cookie",
            };

            commandResult.Apply(context, new StubDataProtector());

            var expectedCookieData = HttpRequestData.ConvertBinaryData(
                StubDataProtector.Protect(commandResult.GetSerializedRequestState()));

            context.Response.StatusCode.Should().Be(302);
            context.Response.Headers["Location"].SingleOrDefault()
                .Should().Be("https://destination.com/", "location header should be set");
            context.Response.Cookies.Received().Append(
                "Saml2.123", expectedCookieData, Arg.Is<CookieOptions>(co => co.HttpOnly));

            context.Response.Cookies.Received().Append(
                "Clear-Cookie", null, Arg.Is<CookieOptions>(co => co.Expires.Value.UtcTicks == 0));

            context.Response.ContentType
                .Should().Be("application/json", "content type should be set");

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            new StreamReader(context.Response.Body).ReadToEnd()
                .Should().Be("{ value: 42 }", "content should be set");
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_Minimal()
        {
            var context = TestHelpers.CreateHttpContext();

            var commandResult = new CommandResult();

            commandResult.Apply(context, new StubDataProtector());

            context.Response.StatusCode.Should().Be(200);
            context.Response.Headers.Keys.Should().NotContain("Location");
            context.Response.Cookies.DidNotReceive().Append(
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CookieOptions>());
        }
    }
}
