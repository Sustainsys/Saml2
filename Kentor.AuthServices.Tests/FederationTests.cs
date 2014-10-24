using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using FluentAssertions;
using System.IdentityModel.Metadata;
using System.Linq;
using Kentor.AuthServices.Configuration;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class FederationTests
    {
        [TestMethod]
        public void Federation_Ctor_NullcheckConfig()
        {
            Action a = () => new Federation(null, Options.FromConfiguration.SPOptions);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("config");
        }

        [TestMethod]
        [DeploymentItem("SambiMetadata.xml")]
        public void Federation_LoadSambiTestMetadata()
        {
            // Sambi is the Swedish health care federation. To test that AuthServices
            // handles some real world metadata, the metadadata from Sambi's test
            // environment is used.

            TestLoadMetadata("SambiMetadata.xml");
        }

        private static void TestLoadMetadata(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var metadata = (EntitiesDescriptor)MetadataLoader.Load(stream);

                Action a = () => new Federation(metadata, true, Options.FromConfiguration.SPOptions);

                a.ShouldNotThrow();
            }
        }

        [TestMethod]
        [DeploymentItem("SkolfederationMetadata.xml")]
        public void Federation_LoadSkolfederationMetadata()
        {
            // Skolfederation is the Swedish national school federation. To test that
            // AuthServices handles some real world metadata, the metdata from the
            // skolfederation federation is used.

            TestLoadMetadata("SkolfederationMetadata.xml");
        }

        [TestMethod]
        public void Federation_Ctor_MetadataUrl()
        {
            var subject = new Federation(
                new Uri("http://localhost:13428/federationMetadata"),
                false,
                Options.FromConfiguration.SPOptions);

            subject.IdentityProviders.First().EntityId.Id.Should().Be("http://idp.federation.example.com/metadata");
        }
    }
}
