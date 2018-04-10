using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Sustainsys.Saml2.Exceptions;
using Sustainsys.Saml2.Saml2P;

namespace Sustainsys.Saml2.Tests.Exceptions
{
    [TestClass]
    public class UnsuccessfulSamlOperationExceptionTest
    {
        [TestMethod]
        public void InvalidSamlOperationException_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<UnsuccessfulSamlOperationException>();
        }

        [TestMethod]
        public void InvalidSamlOperationException_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<UnsuccessfulSamlOperationException>();
        }

        [TestMethod]
        public void InvalidSamlOperationException_StringCtor()
        {
            var msg = "Message!";
            var subject = new UnsuccessfulSamlOperationException(msg);

            subject.Message.Should().Be(msg);
        }

        [TestMethod]
        public void InvalidSamlOperationException_StringInnerExCtor()
        {
            var msg = "Message!";
            var inner = new Exception();
            var subject = new UnsuccessfulSamlOperationException(msg, inner);

            subject.Message.Should().Be(msg);
            subject.InnerException.Should().Be(inner);
        }

        [TestMethod]
        public void InvalidSamlOperationException_SamlStatusCodeCtor()
        {
            var message = "Message!";
            var status = Saml2StatusCode.RequestVersionDeprecated;
            var statusMessage = "Request Version Deprecated";
            var secondLevelStatus = "Second Level Status";

            var subject = new UnsuccessfulSamlOperationException(
                message,
                status,
                statusMessage,
                secondLevelStatus);

            subject.Message.Should().Be("Message!\n" +
                "  Saml2 Status Code: RequestVersionDeprecated\n" +
                "  Saml2 Status Message: Request Version Deprecated\n" +
                "  Saml2 Second Level Status: Second Level Status");
        }
    }
}
