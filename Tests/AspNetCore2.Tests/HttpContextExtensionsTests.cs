using FluentAssertions;
using Sustainsys.Saml2.WebSso;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
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
using Sustainsys.Saml2;
using System.Security.Claims;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    [TestClass]
    public class HttpContextExtensionsTests
    {
        [TestMethod]
        public void HttpContextExtensions_ToHttpRequestData()
        {
            var context = TestHelpers.CreateHttpContext();

            var actual = context.ToHttpRequestData(Substitute.For<ICookieManager>(), StubDataProtector.Unprotect);

            actual.Url.Should().Be(new Uri("https://sp.example.com/somePath?param=value"));
            actual.Form.Count.Should().Be(2);
            actual.Form["Input1"].Should().Be("Value1");
            actual.Form["Input2"].Should().Be("Value2");
            actual.HttpMethod.Should().Be("POST");
            actual.ApplicationUrl.Should().Be(new Uri("https://sp.example.com/"));
        }

        [TestMethod]
        public void HttpContextExtensions_ToHttpRequestData_Get()
        {
            var context = TestHelpers.CreateHttpGet();

            var actual = context.ToHttpRequestData(Substitute.For<ICookieManager>(), StubDataProtector.Unprotect);

            actual.Url.Should().Be(new Uri("https://sp.example.com/somePath?param=value"));
            actual.Form.Count.Should().Be(0);
            actual.HttpMethod.Should().Be("GET");
            actual.ApplicationUrl.Should().Be(new Uri("https://sp.example.com/"));
        }

        [TestMethod]
        public void HttpContextExtensions_ToHttpRequestData_InvalidPost()
        {
            var context = TestHelpers.CreateHttpContext();

            context.Request.HasFormContentType.Returns(false);
            context.Request.Form.Returns(i => { throw new InvalidOperationException(); });

            context.ToHttpRequestData(Substitute.For<ICookieManager>(), StubDataProtector.Unprotect);

            // No exception = pass
        }

        [TestMethod]
        public void HttpContextExtensions_ToHttpRequestData_ApplicationNotInRoot()
        {
            var context = TestHelpers.CreateHttpContext();

            context.Request.PathBase = "/ApplicationPath";

            var actual = context.ToHttpRequestData(Substitute.For<ICookieManager>(), null);

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
            var cookieName = $"{StoredRequestState.CookieNameBase}SomeState";

            var cookieManager = Substitute.For<ICookieManager>();
            cookieManager.GetRequestCookie(context, cookieName).Returns(cookieData);

            var actual = context.ToHttpRequestData(cookieManager, StubDataProtector.Unprotect);

            actual.StoredRequestState.Should().BeEquivalentTo(storedRequestState);
        }

        [TestMethod]
        public void HttpContextExtensions_ToHttpRequestData_HandlesRelayStateWithoutCookie()
        {
            var context = TestHelpers.CreateHttpContext();
            context.Request.QueryString = new QueryString("?RelayState=SomeState");

            context.Invoking(c => c.ToHttpRequestData(Substitute.For<ICookieManager>(), null ))
                .Should().NotThrow();
        }

        [TestMethod]
        public void HttpContextExtensions_ToHttpRequestData_ExtractsUser()
        {
            var context = TestHelpers.CreateHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "NameID")
            }));

            var actual = context.ToHttpRequestData(Substitute.For<ICookieManager>(), null);

            actual.User.Should().BeSameAs(context.User);
        }
    }
}
