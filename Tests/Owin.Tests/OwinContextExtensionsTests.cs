using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Owin;
using FluentAssertions;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sustainsys.Saml2.WebSso;

namespace Sustainsys.Saml2.Owin.Tests
{
    [TestClass]
    public class OwinContextExtensionsTests
    {
        [TestMethod]
        public async Task OwinContextExtensions_ToHttpRequestData_NullYieldsNull()
        {
            IOwinContext ctx = null;

            var actual = await ctx.ToHttpRequestData(null);

            actual.Should().BeNull();
        }

        [TestMethod]
        public async Task OwinContextExtensions_ToHttpRequestData()
        {
            var ctx = OwinTestHelpers.CreateOwinContext();

            ctx.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("Input1=Value1&Input2=Value2"));
            ctx.Request.Method = "POST";
            ctx.Request.ContentType = "application/x-www-form-urlencoded";
            ctx.Request.Path = new PathString("/somePath");
            ctx.Request.QueryString = new QueryString("param=value");

            var actual = await ctx.ToHttpRequestData(StubDataProtector.Unprotect);

            actual.Url.Should().Be(ctx.Request.Uri);
            actual.Form.Count.Should().Be(2);
            actual.Form["Input1"].Should().Be("Value1");
            actual.Form["Input2"].Should().Be("Value2");
            actual.HttpMethod.Should().Be("POST");
            actual.ApplicationUrl.Should().Be(new Uri("http://sp.example.com/"));
        }

        [TestMethod]
        public async Task OwinContextExtensions_ToHttpRequestData_ApplicationNotInRoot()
        {
            var ctx = OwinTestHelpers.CreateOwinContext();

            ctx.Request.PathBase = new PathString("/ApplicationPath");

            var actual = await ctx.ToHttpRequestData(null);

            actual.ApplicationUrl.Should().Be(new Uri("http://sp.example.com/ApplicationPath"));
        }

        [TestMethod]
        public async Task OwinContextExtensions_ToHttpRequestData_ReadsRelayStateCookie()
        {
            var ctx = OwinTestHelpers.CreateOwinContext();
            ctx.Request.QueryString = new QueryString("RelayState", "SomeState");

            var storedRequestState = new StoredRequestState(
                null, new Uri("http://sp.example.com"), null, null);

            var cookieData = HttpRequestData.ConvertBinaryData(
                    StubDataProtector.Protect(storedRequestState.Serialize()));

            ctx.Request.Headers["Cookie"] = $"{StoredRequestState.CookieNameBase}SomeState={cookieData}";

            var actual = await ctx.ToHttpRequestData(StubDataProtector.Unprotect);

            actual.StoredRequestState.Should().BeEquivalentTo(storedRequestState);
        }

        [TestMethod]
        public void OwinContextExtensions_ToHttpRequestData_HandlesRelayStateWithoutCookie()
        {
            var ctx = OwinTestHelpers.CreateOwinContext();
            ctx.Request.QueryString = new QueryString("RelayState", "SomeState");

            ctx.Invoking(async c => await c.ToHttpRequestData(null))
                .Should().NotThrow();
        }
    }
}
