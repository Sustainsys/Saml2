using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using NSubstitute;
using System.Web;
using Sustainsys.Saml2.WebSso;

namespace Sustainsys.Saml2.Tests.WebSso
{
    [TestClass]
    public class NotFoundCommandTests
    {
        [TestMethod]
        public void NotFoundCommand_Run_Sets404()
        {
            var command = new NotFoundCommand();
            var result = command.Run(
                new HttpRequestData("GET", new Uri("http://localhost/Saml2AuthenticationModule/NonExistingPath")),
                null);

            result.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
