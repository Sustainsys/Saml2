using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using System.Web;
using NSubstitute;
using System.Collections.Specialized;

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
        public void AcsCommand_Run_ErrorOnWrongFormatInFormResponse()
        {
            var r = Substitute.For<HttpRequestBase>();
            r.HttpMethod.Returns("POST");
            r.Form.Returns(new NameValueCollection() { { "SAMLResponse", "ABCD" } });

            var cr = new AcsCommand().Run(r);

            var expected = new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.InternalServerError,
                ErrorCode = CommandResultErrorCode.BadFormatSamlResponse
            };

            cr.ShouldBeEquivalentTo(expected);
        }
    }
}
