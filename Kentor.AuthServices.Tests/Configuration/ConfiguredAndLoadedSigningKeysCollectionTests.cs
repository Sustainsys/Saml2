using FluentAssertions;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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
