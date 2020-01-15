using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Sustainsys.Saml2.HttpModule;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.WebSso;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;

namespace Sustainsys.Saml2.HttpModule.Tests
{
	public partial class CommandResultHttpTests
    {
        [TestMethod]
        public void CommandResultHttp_Apply_ChecksResponseNull()
        {
            Action a = () => new CommandResult().Apply(null, true);

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void CommandResultHttp_Apply_NullShouldThrow()
        {
            CommandResult obj = null;
            Action a = () => obj.Apply(null, true);
            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("commandResult");
        }

        [TestMethod]
        public void CommandResultHttp_Apply_HttpStatusCode()
        {
            var response = Substitute.For<HttpResponseBase>();

            new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.PaymentRequired
            }.Apply(response, true);

            response.Received().StatusCode = (int)HttpStatusCode.PaymentRequired;
        }

        [TestMethod]
        public void CommandResultHttp_Apply_SetsCookie()
        {
            var response = Substitute.For<HttpResponseBase>();

            new CommandResult()
            {
                SetCookieName = "CookieName",
                SetCookieSecureFlag = true,
                RequestState = new StoredRequestState(
                    new EntityId("http://idp.example.com"),
                    null,
                    null,
                    null)
            }.Apply(response, true);

            response.Received().SetCookie(
                Arg.Is<HttpCookie>(c => 
                c.Name == "CookieName"
                && c.Value.All(ch => ch != '/' && ch != '+' && ch != '=')
                && new StoredRequestState(DecryptCookieData(c.Value)).Idp.Id == "http://idp.example.com"
                && c.HttpOnly == true
                && c.SameSite == SameSiteMode.None
                && c.Secure == true));
        }

        [TestMethod]
        public void CommandResultHttp_Apply_SetsCookie_NoSameSite()
        {
            var response = Substitute.For<HttpResponseBase>();

            new CommandResult()
            {
                SetCookieName = "CookieName",
                RequestState = new StoredRequestState(
                    new EntityId("http://idp.example.com"),
                    null,
                    null,
                    null)
            }.Apply(response, false);

            response.Received().SetCookie(
                Arg.Is<HttpCookie>(c => c.SameSite == (SameSiteMode)(-1)));
        }

        private byte[] DecryptCookieData(string data)
        {
            return MachineKey.Unprotect(
                HttpRequestData.GetBinaryData(data),
                "Sustainsys.Saml2");
        }

        [TestMethod]
        public void CommandResultHttp_Apply_ClearsCookie()
        {
            var response = Substitute.For<HttpResponseBase>();

            new CommandResult()
            {
                ClearCookieName = "CookieName",
            }.Apply(response, true);

            response.Received().SetCookie(
                Arg.Is<HttpCookie>(c =>
                c.Name == "CookieName"
                && c.Expires == new DateTime(1970, 01, 01)
                && !c.Secure));
        }

        [TestMethod]
        public void CommandResultHttp_Apply_ClearsCookie_SecureFlag()
        {
            var response = Substitute.For<HttpResponseBase>();

            new CommandResult()
            {
                ClearCookieName = "CookieName",
                SetCookieSecureFlag = true
            }.Apply(response, true);

            response.Received().SetCookie(
                Arg.Is<HttpCookie>(c =>
                c.Name == "CookieName"
                && c.Expires == new DateTime(1970, 01, 01)
                && c.Secure));
        }

        [TestMethod]
        public void CommandResultHttp_Apply_Cacheability()
        {
            var cache = Substitute.For<HttpCachePolicyBase>();
            var response = Substitute.For<HttpResponseBase>();
            response.Cache.Returns(cache);

            new CommandResult()
            {
                Cacheability = Cacheability.ServerAndNoCache
            }.Apply(response, true);

            cache.Received().SetCacheability(HttpCacheability.ServerAndNoCache);
        }

        [TestMethod]
        public void CommandResultHttp_Apply_HandleRedirect()
        {
            var response = Substitute.For<HttpResponseBase>();
            var redirectTarget = "http://example.com/redirect/target?X=A%20B%3d";

            new CommandResult()
            {
                Location = new Uri(redirectTarget),
                HttpStatusCode = HttpStatusCode.SeeOther
            }.Apply(response, true);

            response.Received().Redirect(redirectTarget);
        }

        [TestMethod]
        public void CommandResultHttp_Apply_ThrowsOnMissingLocation()
        {
            var response = Substitute.For<HttpResponseBase>();

            Action a = () =>
                new CommandResult()
                {
                    HttpStatusCode = HttpStatusCode.SeeOther
                }.Apply(response, true);

            a.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void CommandResultHttp_Apply_ThrowsOnLocationWithoutRedirect()
        {
            var response = Substitute.For<HttpResponseBase>();

            Action a = () =>
                new CommandResult()
                {
                    Location = new Uri("http://example.com")
                }.Apply(response, true);

            a.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void CommandResultHttp_Apply_Content()
        {
            var response = Substitute.For<HttpResponseBase>();

            var commandResult = new CommandResult
            {
                Content = "Some Content!",
                ContentType = "text"
            };

            commandResult.Apply(response, true);

            response.Received().ContentType = "text";
            response.Received().Write("Some Content!");
        }

        [TestMethod]
        public void CommandResultHttp_Apply_Headers()
        {
            var response = Substitute.For<HttpResponseBase>();

            var commandResult = new CommandResult();

            commandResult.Headers.Add("header", "value");

            commandResult.Apply(response, true);

            response.Received().AddHeader("header", "value");
        }
    }
}