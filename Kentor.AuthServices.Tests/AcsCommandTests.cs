using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using System.Web;
using NSubstitute;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class AcsCommandTests
    {
        [TestMethod]
        public void AcsCommand_Run_ErrorOnNoSamlResponseFound()
        {
            var cr = new AcsCommand().Run(Substitute.For<HttpRequestBase>());

            var expected = new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.InternalServerError,
                ErrorCode = CommandResultErrorCode.NoSamlResponseFound
            };

            cr.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void AcsCommand_Run_ErrorOnWrongXmlInFormResponse()
        {
            Assert.Fail("Fix test - when there is a http post binding that can help");
        }
    }
}
