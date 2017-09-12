using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.WebSso;
using Kentor.AuthServices.HttpModule;
using FluentAssertions;
using NSubstitute;
using System.Web;
using System.Net;
using System.Web.Security;
using System.Text;
using System.Linq;
using System.IdentityModel.Tokens;
using System.IdentityModel.Metadata;

namespace Kentor.AuthServices.Tests.HttpModule
{
    [TestClass]
    public class CommandResultHttpTests
    {
        [TestMethod]
        public void CommandResultHttp_Apply_ChecksResponseNull()
        {
            Action a = () => new CommandResult().Apply(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void CommandResultHttp_Apply_NullShouldThrow()
        {
            CommandResult obj = null;
            Action a = () => obj.Apply(null);
            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("commandResult");
        }

        [TestMethod]
        public void CommandResultHttp_Apply_HttpStatusCode()
        {
            var response = Substitute.For<HttpResponseBase>();

            new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.PaymentRequired
            }.Apply(response);

            response.Received().StatusCode = (int)HttpStatusCode.PaymentRequired;
        }

        [TestMethod]
        public void CommandResultHttp_Apply_SetsCookie()
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
            }.Apply(response);

            response.Received().SetCookie(
                Arg.Is<HttpCookie>(c => 
                c.Name == "CookieName"
                && c.Value.All(ch => ch != '/' && ch != '+' && ch != '=')
                && new StoredRequestState(DecryptCookieData(c.Value)).Idp.Id == "http://idp.example.com"
                && c.HttpOnly == true));
        }

        private byte[] DecryptCookieData(string data)
        {
            return MachineKey.Unprotect(
                HttpRequestData.GetBinaryData(data),
                "Kentor.AuthServices");
        }

        [TestMethod]
        public void CommandResultHttp_Apply_ClearsCookie()
        {
            var response = Substitute.For<HttpResponseBase>();

            new CommandResult()
            {
                ClearCookieName = "CookieName",
            }.Apply(response);

            response.Received().SetCookie(
                Arg.Is<HttpCookie>(c =>
                c.Name == "CookieName"
                && c.Expires == new DateTime(1970, 01, 01)));
        }

        [TestMethod]
        public void CommandResultHttp_ApplyCookies_NullCheck_CommandResult()
        {
            ((CommandResult)null)
                .Invoking(cr => cr.ApplyCookies(Substitute.For<HttpResponseBase>()))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("commandResult");
        }

        [TestMethod]
        public void CommandResultHttp_ApplyCookies_NullCheck_Response()
        {
            new CommandResult()
                .Invoking(cr => cr.ApplyCookies(null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("response");
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
            }.Apply(response);

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
            }.Apply(response);

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
                }.Apply(response);

            a.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void CommandResultHttp_Apply_ThrowsOnLocationWithoutRedirect()
        {
            var response = Substitute.For<HttpResponseBase>();

            Action a = () =>
                new CommandResult()
                {
                    Location = new Uri("http://example.com")
                }.Apply(response);

            a.ShouldThrow<InvalidOperationException>();
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

            commandResult.Apply(response);

            response.Received().ContentType = "text";
            response.Received().Write("Some Content!");
        }

    }
}