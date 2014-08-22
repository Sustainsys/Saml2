using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Kentor.AuthServices.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class CertificatePrivateKeyNotFoundExceptionTests
    {
        [TestMethod]
        public void CertificateNotFoundException_DefaultCtor()
        {
            ExceptionTestHelpers.TestDefaultCtor<CertificatePrivateKeyNotFoundException>();
        }

        [TestMethod]
        public void CertificatePrivateKeyNotFoundException_SerializationCtor()
        {
            ExceptionTestHelpers.TestSerializationCtor<CertificatePrivateKeyNotFoundException>();
        }

        [TestMethod]
        public void CertificatePrivateKeyNotFoundException_CertificateCtor()
        {
            var certificate = SignedXmlHelper.TestCert;
            var msg = String.Format("The private key for the certificate '{0}' was not found or access was denied.", certificate.SubjectName);
            var subject = new CertificatePrivateKeyNotFoundException(certificate);

            subject.Certificate.Should().Be(certificate);
            subject.Message.Should().Be(msg);
        }

        [TestMethod]
        public void CertificatePrivateKeyNotFoundException_StringCtor()
        {
            var certificate = SignedXmlHelper.TestCert;
            var msg = "Message!";
            var subject = new CertificatePrivateKeyNotFoundException(certificate, msg);

            subject.Certificate.Should().Be(certificate);
            subject.Message.Should().Be(msg);
        }

        [TestMethod]
        public void CertificatePrivateKeyNotFoundException_InnerExceptionCtor()
        {
            var certificate = SignedXmlHelper.TestCert;
            var inner = new Exception("inner");
            var msg = "Message!";

            var subject = new CertificatePrivateKeyNotFoundException(certificate, msg, inner);

            subject.Certificate.Should().Be(certificate);
            subject.Message.Should().Be(msg);
            subject.InnerException.Should().Be(inner);
        }
    }
}
