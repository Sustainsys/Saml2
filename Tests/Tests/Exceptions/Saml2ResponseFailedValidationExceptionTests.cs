using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Sustainsys.Saml2.Exceptions;

namespace Sustainsys.Saml2.Tests.Exceptions
{
    [TestClass]
    public class Saml2ResponseFailedValidationExceptionTests
    {
        [TestMethod]
        public void Saml2ResponseFailedValidationException_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<Saml2ResponseFailedValidationException>();
        }

        [TestMethod]
        public void Saml2ResponseFailedValidationException_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<Saml2ResponseFailedValidationException>();
        }

        [TestMethod]
        public void Saml2ResponseFailedValidationException_StringCtor()
        {
            var msg = "Message!";
            var subject = new Saml2ResponseFailedValidationException(msg);

            subject.Message.Should().Be(msg);
        }

        [TestMethod]
        public void Saml2ResponseFailedValidationException_InnerExceptionCtor()
        {
            var msg = "Message!";
            var innerException = new InvalidOperationException("InnerMessage!");

            var subject = new Saml2ResponseFailedValidationException(msg, innerException);

            subject.Message.Should().Be("Message!");
            subject.InnerException.Message.Should().Be("InnerMessage!");
        }
    }
}
