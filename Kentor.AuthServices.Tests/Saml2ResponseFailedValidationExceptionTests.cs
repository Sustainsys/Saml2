using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2ResponseFailedValidationExceptionTests
    {
        [TestMethod]
        public void Saml2ResponseFailedValidationExecption_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<Saml2ResponseFailedValidationException>();
        }

        [TestMethod]
        public void Saml2ResponseFailedValidationExecption_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<Saml2ResponseFailedValidationException>();
        }

        [TestMethod]
        public void Saml2ResponseFailedValidationExecption_StringCtor()
        {
            var msg = "Message!";
            var subject = new Saml2ResponseFailedValidationException(msg);

            subject.Message.Should().Be(msg);
        }
    }
}
