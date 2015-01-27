using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Kentor.AuthServices.Tests.Configuration
{
    [TestClass]
    public class FederationElementTests
    {
        [TestMethod]
        public void FederationElement_SigningCertificate_ShouldLoad()
        {
            var federationElement = new FederationElement();
            federationElement.SigningCertificate = new CertificateElement();
            federationElement.SigningCertificate.AllowConfigEdit(true);
            federationElement.SigningCertificate.FileName = "Kentor.AuthServices.Tests.pfx";

            var testCertificate = new X509Certificate2("Kentor.AuthServices.Tests.pfx");
            var loadedcertificate = federationElement.SigningCertificate.LoadCertificate();

            loadedcertificate.Thumbprint.Should().Be(testCertificate.Thumbprint);
        }
    }
}
