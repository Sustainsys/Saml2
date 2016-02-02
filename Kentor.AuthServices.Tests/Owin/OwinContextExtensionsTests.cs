using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Owin;
using Kentor.AuthServices.Owin;
using FluentAssertions;
using Kentor.AuthServices.Tests.Helpers;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Tests.Owin
{
    [TestClass]
    public class OwinContextExtensionsTests
    {
        [TestMethod]
        public async Task OwinContextExtensionsTests_ToHttpRequestData_NullYieldsNull()
        {
            IOwinContext ctx = null;

            var result = await ctx.ToHttpRequestData(null);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task OwinContextExtensionsTests_ToHttpRequestData()
        {
            var ctx = OwinTestHelpers.CreateOwinContext();

            ctx.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("Input1=Value1&Input2=Value2"));
            ctx.Request.Method = "POST";
            ctx.Request.ContentType = "application/x-www-form-urlencoded";
            ctx.Request.Path = new PathString("/somePath");
            ctx.Request.QueryString = new QueryString("param=value");

            var subject = await ctx.ToHttpRequestData(null);

            subject.Url.Should().Be(ctx.Request.Uri);
            subject.Form.Count.Should().Be(2);
            subject.Form["Input1"].Should().Be("Value1");
            subject.Form["Input2"].Should().Be("Value2");
            subject.HttpMethod.Should().Be("POST");
            subject.ApplicationUrl.Should().Be(new Uri("http://sp.example.com/"));
        }

        [TestMethod]
        public async Task OwinContextExtensionsTests_ToHttpRequestData_ApplicationNotInRoot()
        {
            var ctx = OwinTestHelpers.CreateOwinContext();

            ctx.Request.PathBase = new PathString("/ApplicationPath");

            var subject = await ctx.ToHttpRequestData(null);

            subject.ApplicationUrl.Should().Be(new Uri("http://sp.example.com/ApplicationPath"));
        }


        [TestMethod]
        public async Task OwinContextExtensionsTests_ToHttpRequestData_PublicOrigin()
        {
            var ctx = OwinTestHelpers.CreateOwinContext();
            var options = StubFactory.CreateOptionsPublicOrigin(new Uri("https://my.public.origin.com:8443/"));
            var subject = await ctx.ToHttpRequestData(options);
            subject.ApplicationUrl.Should().Be(new Uri("https://my.public.origin.com:8443/"));
        }
    }
}
