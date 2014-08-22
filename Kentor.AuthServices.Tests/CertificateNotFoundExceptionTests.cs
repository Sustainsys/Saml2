using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class CertificateNotFoundExceptionTests
    {
        [TestMethod]
        public void CertificateNotFoundException_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<CertificateNotFoundException>();
        }

        [TestMethod]
        public void CertificateNotFoundException_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<CertificateNotFoundException>();
        }

        [TestMethod]
        public void CertificateNotFoundException_StringCtor()
        {
            var msg = "Message!";
            var subject = new CertificateNotFoundException(msg);

            subject.Message.Should().Be(msg);
        }

        [TestMethod]
        public void CertificateNotFoundException_InnerExceptionCtor()
        {
            var inner = new Exception("inner");
            var msg = "Message!";

            var subject = new CertificateNotFoundException(msg, inner);

            subject.Message.Should().Be(msg);
            subject.InnerException.Should().Be(inner);
        }
    }
}
