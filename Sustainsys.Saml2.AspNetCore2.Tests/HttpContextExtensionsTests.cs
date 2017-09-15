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
        class FormValues : IFormCollection
        {
            public StringValues this[string key] => throw new NotImplementedException();

            public int Count => throw new NotImplementedException();

            public ICollection<string> Keys => throw new NotImplementedException();

            public IFormFileCollection Files => throw new NotImplementedException();

            public bool ContainsKey(string key)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
            {
                return new[]
                {
                    new KeyValuePair<string, StringValues>("Input1", new StringValues("Value1")),
                    new KeyValuePair<string, StringValues>("Input2", new StringValues("Value2"))
                }.AsEnumerable().GetEnumerator();
            }

            public bool TryGetValue(string key, out StringValues value)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        HttpContext CreateHttpContext()
        {
            var context = Substitute.For<HttpContext>();
            var request = Substitute.For<HttpRequest>();
            context.Request.Returns(request);

            var form = Substitute.For<IFormCollection>();
            context.Request.Form.Returns(new FormValues());
            context.Request.Method = "POST";
            context.Request.ContentType = "application/x-www-form-urlencoded";
            context.Request.Host = new HostString("sp.example.com");
            context.Request.Scheme = "https";
            context.Request.Path = new PathString("/somePath");
            context.Request.PathBase = new PathString();
            context.Request.QueryString = new QueryString("?param=value");

            return context;
        }

        [TestMethod]
        public async Task HttpContextExtensions_ToHttpRequestData()
        {
            var context = CreateHttpContext();
            var actual = await context.ToHttpRequestData(StubDataProtector.Unprotect);

            actual.Url.Should().Be(new Uri("https://sp.example.com/somePath"));
            actual.Form.Count.Should().Be(2);
            actual.Form["Input1"].Should().Be("Value1");
            actual.Form["Input2"].Should().Be("Value2");
            actual.HttpMethod.Should().Be("POST");
            actual.ApplicationUrl.Should().Be(new Uri("https://sp.example.com/"));
        }

        [TestMethod]
        public async Task HttpContextExtensions_ToHttpRequestData_ApplicationNotInRoot()
        {
            var context = CreateHttpContext();

            context.Request.PathBase = new PathString("/ApplicationPath");

            var actual = await context.ToHttpRequestData(null);

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
