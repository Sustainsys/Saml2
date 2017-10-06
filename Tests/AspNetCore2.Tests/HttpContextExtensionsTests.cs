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
using Kentor.AuthServices;

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

            actual.Url.Should().Be(new Uri("https://sp.example.com/somePath?param=value"));
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

            context.Request.PathBase = "/ApplicationPath";

            var actual = context.ToHttpRequestData(null);

            actual.ApplicationUrl.Should().Be(new Uri("https://sp.example.com/ApplicationPath"));
        }

        [TestMethod]
        public void HttpContextExtensions_ToHttpRequestData_ReadsRelayStateCookie()
        {
            var context = TestHelpers.CreateHttpContext();
            context.Request.QueryString = new QueryString("?RelayState=SomeState");
           
            var storedRequestState = new StoredRequestState(
                null, new Uri("http://sp.example.com"), null, null);

            var cookieData = HttpRequestData.ConvertBinaryData(
                    StubDataProtector.Protect(storedRequestState.Serialize()));

            context.Request.Cookies = new StubCookieCollection(
                Enumerable.Repeat(new KeyValuePair<string, string>(
                    $"Kentor.SomeState", cookieData), 1));

            var actual = context.ToHttpRequestData(StubDataProtector.Unprotect);

            actual.StoredRequestState.ShouldBeEquivalentTo(storedRequestState);
        }

        [TestMethod]
        public void OwinContextExtensions_ToHttpRequestData_HandlesRelayStateWithoutCookie()
        {
            var context = TestHelpers.CreateHttpContext();
            context.Request.QueryString = new QueryString("?RelayState=SomeState");

            context.Invoking(c => c.ToHttpRequestData(null))
                .ShouldNotThrow();
        }
    }
}
