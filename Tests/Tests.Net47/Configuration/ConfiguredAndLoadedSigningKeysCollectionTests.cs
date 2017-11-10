using FluentAssertions;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Kentor.AuthServices.Tests.Configuration
{
    [TestClass]
    public class ConfiguredAndLoadedSigningKeysCollectionTests
    {
        [TestMethod]
        public void ConfiguredAndLoadedSigningKeysCollection_AddConfiguredKey()
        {
            var subject = new ConfiguredAndLoadedSigningKeysCollection();

            subject.AddConfiguredKey(SignedXmlHelper.TestCert);

            new X509Certificate2(subject.Single()
                .As<X509RawDataKeyIdentifierClause>()
                .GetX509RawData()).Thumbprint
                .Should().Be(SignedXmlHelper.TestCert.Thumbprint);
        }
    }
}
