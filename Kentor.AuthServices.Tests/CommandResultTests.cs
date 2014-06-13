using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using NSubstitute;
using System.Web;
using System.Security.Claims;
using System.Security.Principal;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class CommandResultTests
    {
        [TestMethod]
        public void CommandResult_Defaults()
        {
            var expected = new
            {
                HttpStatusCode = HttpStatusCode.OK,
                Cacheability = HttpCacheability.NoCache,
                Location = (Uri)null,
                Principal = (ClaimsPrincipal)null,
                ContentType = (string)null,
                Content = (string)null
            };

            new CommandResult().ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void CommandResult_Apply_ChecksResponseNull()
        {
            Action a = () => new CommandResult().Apply(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void CommandResult_Apply_HttpStatusCode()
        {
            var response = Substitute.For<HttpResponseBase>();

            new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.PaymentRequired
            }.Apply(response);

            response.Received().StatusCode = (int)HttpStatusCode.PaymentRequired;
        }

        [TestMethod]
        public void CommandResult_Apply_Cacheability()
        {
            var cache = Substitute.For<HttpCachePolicyBase>();
            var response = Substitute.For<HttpResponseBase>();
            response.Cache.Returns(cache);

            new CommandResult()
            {
                Cacheability = HttpCacheability.ServerAndNoCache
            }.Apply(response);

            cache.Received().SetCacheability(HttpCacheability.ServerAndNoCache);
        }

        [TestMethod]
        public void CommandResult_Apply_HandleRedirect()
        {
            var response = Substitute.For<HttpResponseBase>();
            var redirectTarget = "http://example.com/redirect/target/";

            new CommandResult()
            {
                Location = new Uri(redirectTarget),
                HttpStatusCode = HttpStatusCode.SeeOther
            }.Apply(response);

            response.Received().Redirect(redirectTarget);
        }

        [TestMethod]
        public void CommandResult_Apply_ThrowsOnMissingLocation()
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
        public void CommandResult_Apply_ThrowsOnLocationWithoutRedirect()
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
        public void Command_Result_Apply_Content()
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
