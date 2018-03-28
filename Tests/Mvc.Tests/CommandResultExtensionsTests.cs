using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Web.Mvc;
using Sustainsys.Saml2.WebSso;

namespace Sustainsys.Saml2.Mvc.Tests
{
    [TestClass]
    public class CommandResultExtensionsTests
    {
        [TestMethod]
        public void CommandResultExtensions_ToActionResult_NullCheck()
        {
            Action a = () => ((CommandResult)null).ToActionResult();

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("commandResult");
        }

        [TestMethod]
        public void CommandResultExtensions_ToActionResult_SeeOther()
        {
            var strLocation = "http://example.com/location?X=A%20B%3DZ";

            var cr = new CommandResult()
            {
                HttpStatusCode = System.Net.HttpStatusCode.SeeOther,
                Location = new Uri(strLocation)
            };

            var subject = cr.ToActionResult();

            subject.Should().BeOfType<RedirectResult>().And
                .Subject.As<RedirectResult>().Url.Should().Be(strLocation);
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

            a.Should().Throw<NotImplementedException>();
        }
    }
}
