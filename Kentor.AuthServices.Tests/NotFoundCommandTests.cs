using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using NSubstitute;
using System.Web;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class NotFoundCommandTests
    {
        [TestMethod]
        public void NotFoundCommand_Run_Sets404()
        {
            var command = new NotFoundCommand();
            var result = command.Run(Substitute.For<HttpRequestBase>());

            result.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
