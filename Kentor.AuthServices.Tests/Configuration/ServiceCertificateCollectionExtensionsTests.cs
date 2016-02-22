using FluentAssertions;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Tests.Configuration
{
    [TestClass]
    public class ServiceCertificateCollectionExtensionsTests
    {
        [TestMethod]
        public void ServiceCertificateCollectionExtensions_Add()
        {
            Action a = () => ((ICollection<ServiceCertificate>)null).Add(SignedXmlHelper.TestCert);

            a.ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("collection");
        }
    }
}
