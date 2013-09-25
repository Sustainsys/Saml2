using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using System.Web;
using NSubstitute;
using System.Collections.Specialized;
using System.Text;

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
        public void AcsCommand_Run_ErrorOnNotBase64InFormResponse()
        {
            var r = Substitute.For<HttpRequestBase>();
            r.HttpMethod.Returns("POST");
            r.Form.Returns(new NameValueCollection() { { "SAMLResponse", "#¤!2" } });

            var cr = new AcsCommand().Run(r);

            var expected = new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.InternalServerError,
                ErrorCode = CommandResultErrorCode.BadFormatSamlResponse
            };

            cr.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void AcsCommand_Run_ErrorOnIncorrectXml()
        {
            var r = Substitute.For<HttpRequestBase>();
            r.HttpMethod.Returns("POST");
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("<foo />"));
            r.Form.Returns(new NameValueCollection() { { "SAMLResponse", encoded } });

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
