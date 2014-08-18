using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Mvc;
using FluentAssertions;
using System.Web.Mvc;
using Kentor.AuthServices.TestHelpers;
using Kentor.AuthServices.Owin;
using System.IO;
using System.Text;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class CommandResultExtensionsTests
    {
        [TestMethod]
        public void CommandResultExtensions_ToActionResult_NullCheck()
        {
            Action a = () => ((CommandResult)null).ToActionResult();

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("commandResult");
        }

        [TestMethod]
        public void CommandResultExtensions_ToActionResult_SeeOther()
        {
            var cr = new CommandResult()
            {
                HttpStatusCode = System.Net.HttpStatusCode.SeeOther,
                Location = new Uri("http://example.com/location")
            };

            var subject = cr.ToActionResult();

            subject.Should().BeOfType<RedirectResult>().And
                .Subject.As<RedirectResult>().Url.Should().Be("http://example.com/location");
        }

        [TestMethod]
        public void CommandResultExtensions_ToActionResult_Ok()
        {
            var cr = new CommandResult()
            {
                Content = "Some Content!",
                ContentType = "application/whatever+text"
            };

            var subject = cr.ToActionResult();

            subject.Should().BeOfType<ContentResult>().And
                .Subject.As<ContentResult>().Content.Should().Be(cr.Content);

            subject.As<ContentResult>().ContentType.Should().Contain(cr.ContentType);
        }

        [TestMethod]
        public void CommandResultExtensions_ToActionResult_UknownStatusCode()
        {
            var cr = new CommandResult()
            {
                HttpStatusCode = System.Net.HttpStatusCode.SwitchingProtocols
            };

            Action a = () => cr.ToActionResult();

            a.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_NullCheck_CommandResult()
        {
            Action a = () => ((CommandResult)null).Apply(OwinTestHelpers.CreateOwinContext());

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("commandResult");
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_NullCheck_OwinContext()
        {
            Action a = () => new CommandResult().Apply(context:null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("context");
        }

        [TestMethod]
        public void CommandResultExtensions_Apply_Ok()
        {
            var cr = new CommandResult()
            {
                Content = "Some Content!",
                ContentType = "application/whatever+text"
            };

            var context = OwinTestHelpers.CreateOwinContext();

            cr.Apply(context);

            context.Response.StatusCode.Should().Be(200);
            context.Response.ContentType.Should().Be("application/whatever+text");
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(context.Response.Body))
            {
                var bodyText = reader.ReadToEnd();
                bodyText.Should().Be("Some Content!");
            }
        }
    }
}
