using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Configuration;
using FluentAssertions;

namespace Kentor.AuthServices.Tests.Configuration
{
    [TestClass]
    public class ServiceCertificateCollectionTests
    {
        [TestMethod]
        public void ServiceCertificateCollection_RegisterCerts_NullCheck()
        {
            var subject = new ServiceCertificateCollection();

            Action a = () => subject.RegisterServiceCertificates(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }
    }
}
