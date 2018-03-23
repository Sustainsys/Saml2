using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sustainsys.Saml2.Tests
{
    [TestClass]
    public class ServiceCertificateTests
    {
        [TestMethod]
        public void ServiceCertificate_DefaultCtor()
        {
            var cert = new ServiceCertificate();
            cert.Status.Should().Be(CertificateStatus.Current);
            cert.Use.Should().Be(CertificateUse.Both);
            cert.MetadataPublishOverride.Should().Be(MetadataPublishOverrideType.None);
        }

        [TestMethod]
        public void ServiceCertificate_ConfigCtor()
        {
            Action a = () => new ServiceCertificate(serviceCertElement: null);
            a.Should().Throw<ArgumentNullException>();
        }
    }
}
