using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Sustainsys.Saml2.Exceptions;
using Sustainsys.Saml2.Metadata.Exceptions;

namespace Sustainsys.Saml2.Tests.Exceptions
{
    [TestClass]
    public class InvalidSignatureExceptionTests
    {
        [TestMethod]
        public void InvalidSignatureException_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<InvalidSignatureException>();
        }

        [TestMethod]
        public void InvalidSignatureException_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<InvalidSignatureException>();
        }

        [TestMethod]
        public void InvalidSignatureException_StringCtor()
        {
            var msg = "Message!";
            var subject = new InvalidSignatureException(msg);

            subject.Message.Should().Be(msg);
        }

        [TestMethod]
        public void InvalidSignatureException_InnerExceptionCtor()
        {
            var inner = new Exception("inner");
            var msg = "Message!";

            var subject = new InvalidSignatureException(msg, inner);

            subject.Message.Should().Be(msg);
            subject.InnerException.Should().Be(inner);
        }
    }
}
