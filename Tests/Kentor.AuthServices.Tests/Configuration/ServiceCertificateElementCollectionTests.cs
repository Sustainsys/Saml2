using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Configuration;
using FluentAssertions;

namespace Kentor.AuthServices.Tests.Configuration
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
