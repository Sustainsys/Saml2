using FluentAssertions;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.WebSso;
using System;
using System.IO;
using System.Net;

namespace Sustainsys.Saml2.Owin.Tests
{
	[TestClass]
    public class CommandResultExtensionsTests
    {
        [TestMethod]
        public void CommandResultExtensions_Apply_NullCheck_CommandResult()
        {
            Action a = () => ((CommandResult)null).Apply(OwinTestHelpers.CreateOwinContext(), null, true);

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("commandResult");
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_NullCheck_OwinContext()
        {
            Action a = () => new CommandResult().Apply(context:null, dataProtector:null, emitSameSiteNone:true);

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("context");
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_Content()
        {
            var cr = new CommandResult()
            {
                Content = "Some Content!",
                ContentType = "application/whatever+text"
            };

            var context = OwinTestHelpers.CreateOwinContext();

            cr.Apply(context, null, true);

            context.Response.StatusCode.Should().Be(200);
            context.Response.ContentType.Should().Be("application/whatever+text");
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(context.Response.Body))
            {
                var bodyText = reader.ReadToEnd();
                bodyText.Should().Be("Some Content!");
            }
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_Cookie()
        {
            var cr = new CommandResult()
            {
                RequestState = new StoredRequestState(
                    new EntityId("http://idp.example.com"),
                    new Uri("http://sp.example.com/loggedout"),
                    new Saml2Id("id123"),
                    null),
                SetCookieName = "CookieName",
                SetCookieSecureFlag = false,
            };

            var context = OwinTestHelpers.CreateOwinContext();

            var dataProtector = new StubDataProtector();
            cr.Apply(context, dataProtector, false);

            var setCookieHeader = context.Response.Headers["Set-Cookie"];

            var protectedData = HttpRequestData.ConvertBinaryData(
                StubDataProtector.Protect(cr.GetSerializedRequestState()));

            var expected = $"CookieName={protectedData}; path=/; HttpOnly";

            setCookieHeader.Should().Be(expected);
        }

        [TestMethod]
        public void CommandREsultExtensions_Apply_Cookie_EmitSameSiteNone_AndSecure()
        {
            var cr = new CommandResult()
            {
                RequestState = new StoredRequestState(
                    new EntityId("http://idp.example.com"),
                    new Uri("http://sp.example.com/loggedout"),
                    new Saml2Id("id123"),
                    null),
                SetCookieName = "CookieName",
                SetCookieSecureFlag = true,
            };

            var context = OwinTestHelpers.CreateOwinContext();

            var dataProtector = new StubDataProtector();
            cr.Apply(context, dataProtector, true);

            var setCookieHeader = context.Response.Headers["Set-Cookie"];

            var protectedData = HttpRequestData.ConvertBinaryData(
                StubDataProtector.Protect(cr.GetSerializedRequestState()));

            var expected = $"CookieName={protectedData}; path=/; secure; HttpOnly; SameSite=None";

            setCookieHeader.Should().Be(expected);
        }

        [TestMethod]
        public void CommandResultExtensions_DoesNotApplyCookieWhenNoNameSet()
        {
            var cr = new CommandResult()
            {
                RequestState = new StoredRequestState(
                    new EntityId("http://idp.example.com"),
                    new Uri("http://sp.example.com/loggedout"),
                    new Saml2Id("id123"),
                    null),
                SetCookieName = null
            };

            var context = OwinTestHelpers.CreateOwinContext();

            var dataProtector = new StubDataProtector();
            cr.Apply(context, dataProtector, true);

            var setCookieHeader = context.Response.Headers["Set-Cookie"];

            var protectedData = HttpRequestData.ConvertBinaryData(
                StubDataProtector.Protect(cr.GetSerializedRequestState()));

            setCookieHeader.Should().Be(null);
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_ClearCookie()
        {
            var cr = new CommandResult()
            {
                ClearCookieName = "CookieName"
            };

            var context = OwinTestHelpers.CreateOwinContext();
            var dataProtector = new StubDataProtector();
            cr.Apply(context, dataProtector, true);

            var setCookieHeader = context.Response.Headers["Set-Cookie"];

            var expected = "CookieName=; path=/; expires=Thu, 01-Jan-1970 00:00:00 GMT";

            setCookieHeader.Should().Be(expected);
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_ClearCookie_Secure()
        {
            var cr = new CommandResult()
            {
                ClearCookieName = "CookieName",
                SetCookieSecureFlag = true
            };

            var context = OwinTestHelpers.CreateOwinContext();
            var dataProtector = new StubDataProtector();
            cr.Apply(context, dataProtector, true);

            var setCookieHeader = context.Response.Headers["Set-Cookie"];

            var expected = "CookieName=; path=/; expires=Thu, 01-Jan-1970 00:00:00 GMT; secure";

            setCookieHeader.Should().Be(expected);
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_Redirect()
        {
            string redirectUrl = "http://somewhere.else.example.com?Foo=Bar%20XYZ";
            var cr = new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = new Uri(redirectUrl)
            };

            var context = OwinTestHelpers.CreateOwinContext();

            cr.Apply(context, null, true);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().Be(redirectUrl);
        }

        [TestMethod]
        public void CommandResult_Apply_Headers()
        {
            var cr = new CommandResult();
            cr.Headers.Add("header", "value");

            var context = OwinTestHelpers.CreateOwinContext();

            cr.Apply(context, null, true);

            context.Response.Headers["header"].Should().Be("value");
        }
    }
}
