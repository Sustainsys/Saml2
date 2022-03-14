using FluentAssertions;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Sustainsys.Saml2.Tests.Configuration
{
    [TestClass]
    public class ServiceCertificateCollectionTests
    {
        [TestMethod]
        public void ServiceCertificateCollection_Add_NullItem()
        {
            var subject = new ServiceCertificateCollection();

            Action a = () => subject.Add((ServiceCertificate)null);

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("item");
        }

        [TestMethod]
        public void ServiceCertificateCollection_Add_NullCert()
        {
            var subject = new ServiceCertificateCollection();

            Action a = () => subject.Add((X509Certificate2)null);

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("certificate");
        }

        [TestMethod]
        public void ServiceCertificateCollection_Add_FailsWhenNoPrivateKey_Enc()
        {
            var certBytes = SignedXmlHelper.TestCert.Export(X509ContentType.Cert);
            var certWithNoPrivate = new X509Certificate2(certBytes);

            var subject = new ServiceCertificateCollection();
            Action a = () => subject.Add(new ServiceCertificate { Use = CertificateUse.Encryption, Certificate = certWithNoPrivate });

            a.Should().Throw<ArgumentException>()
                .WithMessage(@"Provided certificate is not valid because it does not contain a private key.");
        }

        [TestMethod]
        public void ServiceCertificateCollection_Add_FailsWhenNoPrivateKey_Sign()
        {
            var certBytes = SignedXmlHelper.TestCertSignOnly.Export(X509ContentType.Cert);
            var certWithNoPrivate = new X509Certificate2(certBytes);

            var subject = new ServiceCertificateCollection();
            Action a = () => subject.Add(new ServiceCertificate { Use = CertificateUse.Signing, Certificate = certWithNoPrivate });

            a.Should().Throw<ArgumentException>()
                .WithMessage(@"Provided certificate is not valid because it does not contain a private key.");
        }

        [TestMethod]
        public void ServiceCertificateCollection_Add_FailsWhenNoPrivateKey_Both()
        {
            var certBytes = SignedXmlHelper.TestCert.Export(X509ContentType.Cert);
            var certWithNoPrivate = new X509Certificate2(certBytes);

            var subject = new ServiceCertificateCollection();
            Action a = () => subject.Add(certWithNoPrivate);

            a.Should().Throw<ArgumentException>()
                .WithMessage(@"Provided certificate is not valid because it does not contain a private key.");
        }

        [TestMethod]
        public void ServiceCertificateCollection_Add_EncryptionCert_FailsWhenNotValid()
        {
            var subject = new ServiceCertificateCollection();
            Action a = () => subject.Add(new ServiceCertificate { Use = CertificateUse.Encryption, Certificate = SignedXmlHelper.TestCertSignOnly });

            a.Should().Throw<ArgumentException>()
                .WithMessage( @"Provided certificate is not valid for encryption/decryption. There may be insufficient permissions to its private key in the windows certificate store or the certificate itself may not have the correct purposes. If you only want to use it for signing, set the Use property to Signing (CertificateUse.Signing)." );
        }

        [TestMethod]
        public void ServiceCertificateCollection_Add_EncryptionCert_WorksWhenValid()
        {
            var subject = new ServiceCertificateCollection();
            subject.Add(new ServiceCertificate { Use = CertificateUse.Encryption, Certificate = SignedXmlHelper.TestCert2 });

            subject.Count.Should().Be(1);
        }

        [TestMethod]
        public void ServiceCertificateCollection_Add_SigningCert_Works()
        {
            var subject = new ServiceCertificateCollection();
            subject.Add(new ServiceCertificate { Use = CertificateUse.Signing, Certificate = SignedXmlHelper.TestCertSignOnly });

            subject.Count.Should().Be(1);
        }
    }
}
