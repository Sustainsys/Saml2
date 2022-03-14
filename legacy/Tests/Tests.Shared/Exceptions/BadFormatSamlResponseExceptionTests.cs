using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Sustainsys.Saml2.Exceptions;

namespace Sustainsys.Saml2.Tests.Exceptions
{
    [TestClass]
    public class BadFormatSamlResponseExceptionTests
    {
        [TestMethod]
        public void BadFormatSamlResponseException_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<BadFormatSamlResponseException>();
        }

        [TestMethod]
        public void BadFormatSamlResponseException_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<BadFormatSamlResponseException>();
        }

        [TestMethod]
        public void BadFormatSamlResponseException_StringCtor()
        {
            var msg = "Message!";
            var subject = new BadFormatSamlResponseException(msg);

            subject.Message.Should().Be(msg);
        }
    }
}
