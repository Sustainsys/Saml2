using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using NSubstitute;
using System.Web;
using System.Security.Claims;

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
                ErrorCode = CommandResultErrorCode.NoError,
                Principal = (ClaimsPrincipal)null
            };

            new CommandResult().ShouldBeEquivalentTo(expected);
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
    }
}
