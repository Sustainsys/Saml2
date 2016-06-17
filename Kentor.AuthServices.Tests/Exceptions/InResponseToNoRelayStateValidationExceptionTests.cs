using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Kentor.AuthServices.Exceptions;

namespace Kentor.AuthServices.Tests.Exceptions
{
    [TestClass]
    public class InResponseToNoRelayStateValidationExceptionTests
    {
        [TestMethod]
        public void InResponseToNoRelayStateValidationException_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<InResponseToNoRelayStateValidationException>();
        }

        [TestMethod]
        public void InResponseToNoRelayStateValidationException_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<InResponseToNoRelayStateValidationException>();
        }

        [TestMethod]
        public void InResponseToNoRelayStateValidationException_StringCtor()
        {
            var msg = "Message!";
            var subject = new InResponseToNoRelayStateValidationException(msg);

            subject.Message.Should().Be(msg);
        }

        [TestMethod]
        public void InResponseToNoRelayStateValidationException_InnerExceptionCtor()
        {
            var msg = "Message!";
            var innerException = new InvalidOperationException("InnerMessage!");

            var subject = new InResponseToNoRelayStateValidationException(msg, innerException);

            subject.Message.Should().Be("Message!");
            subject.InnerException.Message.Should().Be("InnerMessage!");
        }
    }
}
