using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Configuration;
using FluentAssertions;

namespace Sustainsys.Saml2.Tests.Configuration
{
    [TestClass]
    public class ServiceCertificateElementCollectionTests
    {
        [TestMethod]
        public void ServiceCertificateElementCollection_RegisterCerts_NullCheck()
        {
            var subject = new ServiceCertificateElementCollection();

            Action a = () => subject.RegisterServiceCertificates(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }
    }
}
