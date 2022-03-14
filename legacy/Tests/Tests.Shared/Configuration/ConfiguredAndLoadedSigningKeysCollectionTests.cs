using FluentAssertions;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Sustainsys.Saml2.Metadata.Tokens;

namespace Sustainsys.Saml2.Tests.Configuration
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
