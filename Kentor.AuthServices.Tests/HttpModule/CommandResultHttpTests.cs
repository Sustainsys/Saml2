using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.WebSso;
using Kentor.AuthServices.HttpModule;
using FluentAssertions;
using NSubstitute;
using System.Web;
using System.Net;

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
