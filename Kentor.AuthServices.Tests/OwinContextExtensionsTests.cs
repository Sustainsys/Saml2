using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Owin;
using Kentor.AuthServices.Owin;
using FluentAssertions;
using Kentor.AuthServices.TestHelpers;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class OwinContextExtensionsTests
    {
        [TestMethod]
        public async Task OwinContextExtensionsTests_ToHttpRequestData_NullYieldsNull()
        {
            IOwinContext ctx = null;

            var result = await ctx.ToHttpRequestData();

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task OwinContextExtensionsTests_ToHttpRequestData()
        {
            IOwinContext ctx = OwinTestHelpers.CreateOwinContext();

            ctx.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("Input1=Value1&Input2=Value2"));
            ctx.Request.Method = "POST";
            ctx.Request.ContentType = "application/x-www-form-urlencoded";

            var subject = await ctx.ToHttpRequestData();

            subject.Url.Should().Be(ctx.Request.Uri);
            subject.Form.Count.Should().Be(2);
            subject.Form["Input1"].Should().Be("Value1");
            subject.Form["Input2"].Should().Be("Value2");
            subject.HttpMethod.Should().Be("POST");
        }
    }
}
