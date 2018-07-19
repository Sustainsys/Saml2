using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Sustainsys.Saml2.Exceptions;

namespace Sustainsys.Saml2.Tests.Exceptions
{
    [TestClass]
    public class UnexpectedInResponseToExceptionTests
    {
        [TestMethod]
        public void UnexpectedInResponseToException_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<UnexpectedInResponseToException>();
        }

        [TestMethod]
        public void UnexpectedInResponseToException_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<UnexpectedInResponseToException>();
        }

        [TestMethod]
        public void UnexpectedInResponseToException_StringCtor()
        {
            var msg = "Message!";
            var subject = new UnexpectedInResponseToException(msg);

            subject.Message.Should().Be(msg);
        }

        [TestMethod]
        public void UnexpectedInResponseToException_InnerExceptionCtor()
        {
            var msg = "Message!";
            var innerException = new InvalidOperationException("InnerMessage!");

            var subject = new UnexpectedInResponseToException(msg, innerException);

            subject.Message.Should().Be("Message!");
            subject.InnerException.Message.Should().Be("InnerMessage!");
        }
    }
}
