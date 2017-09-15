using FluentAssertions;
using Kentor.AuthServices.WebSso;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    [TestClass]
    public class HttpContextExtensionsTests
    {
        [TestMethod]
        public void HttpContextExtensions_ToHttpRequestData()
        {
            var context = TestHelpers.CreateHttpContext();
            var actual = context.ToHttpRequestData(StubDataProtector.Unprotect);

            actual.Url.Should().Be(new Uri("https://sp.example.com/somePath"));
            actual.Form.Count.Should().Be(2);
            actual.Form["Input1"].Should().Be("Value1");
            actual.Form["Input2"].Should().Be("Value2");
            actual.HttpMethod.Should().Be("POST");
            actual.ApplicationUrl.Should().Be(new Uri("https://sp.example.com/"));
        }

        [TestMethod]
        public void HttpContextExtensions_ToHttpRequestData_ApplicationNotInRoot()
        {
            var context = TestHelpers.CreateHttpContext();

            context.Request.PathBase = new PathString("/ApplicationPath");

            var actual = context.ToHttpRequestData(null);

            actual.ApplicationUrl.Should().Be(new Uri("https://sp.example.com/ApplicationPath"));
        }

        //[TestMethod]
        //public async Task OwinContextExtensionsTests_ToHttpRequestData_ReadsRelayStateCookie()
        //{
        //    var ctx = OwinTestHelpers.CreateOwinContext();
        //    ctx.Request.QueryString = new QueryString("RelayState", "SomeState");

        //    var storedRequestState = new StoredRequestState(
        //        null, new Uri("http://sp.example.com"), null, null);

        //    var cookieData = HttpRequestData.ConvertBinaryData(
        //            StubDataProtector.Protect(storedRequestState.Serialize()));

        //    ctx.Request.Headers["Cookie"] = $"Kentor.SomeState={cookieData}";

        //    var actual = await ctx.ToHttpRequestData(StubDataProtector.Unprotect);

        //    actual.StoredRequestState.ShouldBeEquivalentTo(storedRequestState);
        //}

        //[TestMethod]
        //public void OwinContextExtensionsTests_ToHttpRequestData_HandlesRelayStateWithoutCookie()
        //{
        //    var ctx = OwinTestHelpers.CreateOwinContext();
        //    ctx.Request.QueryString = new QueryString("RelayState", "SomeState");

        //    ctx.Invoking(async c => await c.ToHttpRequestData(null))
        //        .ShouldNotThrow();
        //}

    }
}
