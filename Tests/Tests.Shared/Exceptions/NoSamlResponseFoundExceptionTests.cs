using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Sustainsys.Saml2.Exceptions;

namespace Sustainsys.Saml2.Tests.Exceptions
{
    [TestClass]
    public class NoSamlResponseFoundExceptionTests
    {
        [TestMethod]
        public void NoSamlResponseFoundException_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<NoSamlResponseFoundException>();
        }

        [TestMethod]
        public void NoSamlResponseFoundException_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<NoSamlResponseFoundException>();
        }

        [TestMethod]
        public void NoSamlResponseFoundException_StringCtor()
        {
            var msg = "Message!";
            var subject = new NoSamlResponseFoundException(msg);

            subject.Message.Should().Be(msg);
        }

        [TestMethod]
        public void NoSamlResponseFoundExceptoin_InnerExceptionCtor()
        {
            var inner = new Exception("inner");
            var msg = "Message!";

            var subject = new NoSamlResponseFoundException(msg, inner);

            subject.Message.Should().Be(msg);
            subject.InnerException.Should().Be(inner);
        }
    }
}
