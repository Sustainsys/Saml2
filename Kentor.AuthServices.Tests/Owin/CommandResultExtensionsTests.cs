using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Mvc;
using FluentAssertions;
using System.Web.Mvc;
using Kentor.AuthServices.Tests.Helpers;
using Kentor.AuthServices.Owin;
using System.IO;
using System.Text;
using System.Net;
using Kentor.AuthServices.WebSso;
using Microsoft.Owin.Security.DataProtection;
using System.Linq;

namespace Kentor.AuthServices.Tests.Owin
{
    [TestClass]
    public class CommandResultExtensionsTests
    {
        [TestMethod]
        public void CommandResultExtensions_Apply_NullCheck_CommandResult()
        {
            Action a = () => ((CommandResult)null).Apply(OwinTestHelpers.CreateOwinContext(), null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("commandResult");
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_NullCheck_OwinContext()
        {
            Action a = () => new CommandResult().Apply(context:null, dataProtector:null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("context");
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_Content()
        {
            var cr = new CommandResult()
            {
                Content = "Some Content!",
                ContentType = "application/whatever+text"
            };

            var context = OwinTestHelpers.CreateOwinContext();

            cr.Apply(context, null);

            context.Response.StatusCode.Should().Be(200);
            context.Response.ContentType.Should().Be("application/whatever+text");
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(context.Response.Body))
            {
                var bodyText = reader.ReadToEnd();
                bodyText.Should().Be("Some Content!");
            }
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_Cookie()
        {
            var cr = new CommandResult()
            {
                SetCookieData = "???>>>SomeData",
                SetCookieName = "CookieName"
            };

            var context = OwinTestHelpers.CreateOwinContext();

            var dataProtector = new StubDataProtector();
            cr.Apply(context, dataProtector);

            var setCookieHeader = context.Response.Headers["Set-Cookie"];

            var protectedData = StubDataProtector.Protect(cr.SetCookieData);

            var expected = $"CookieName={protectedData}; path=/; HttpOnly";

            setCookieHeader.Should().Be(expected);
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_Redirect()
        {
            string redirectUrl = "http://somewhere.else.example.com?Foo=Bar%20XYZ";
            var cr = new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = new Uri(redirectUrl)
            };

            var context = OwinTestHelpers.CreateOwinContext();

            cr.Apply(context, null);

            context.Response.StatusCode.Should().Be(303);
            context.Response.Headers["Location"].Should().Be(redirectUrl);
        }
    }
}
