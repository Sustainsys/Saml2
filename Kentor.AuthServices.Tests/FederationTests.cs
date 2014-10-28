using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using FluentAssertions;
using System.IdentityModel.Metadata;
using System.Linq;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.TestHelpers;
using Kentor.AuthServices.Tests.Metadata;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class FederationTests
    {
        TimeSpan refreshMinInterval = MetadataRefreshScheduler.minInternval;

        [TestCleanup]
        public void Cleanup()
        {
            MetadataServer.IdpVeryShortCacheDurationIncludeInvalidKey = false;
            MetadataRefreshScheduler.minInternval = refreshMinInterval;
        }

        [TestMethod]
        public void Federation_Ctor_NullcheckConfig()
        {
            Action a = () => new Federation(null, Options.FromConfiguration.SPOptions);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("config");
        }

        [TestMethod]
        public void Federation_LoadSambiTestMetadata()
        {
            // Sambi is the Swedish health care federation. To test that AuthServices
            // handles some real world metadata, the metadadata from Sambi's test
            // environment is used.

            TestLoadMetadata("Metadata\\SambiMetadata.xml");
        }

        private static void TestLoadMetadata(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var metadata = (ExtendedEntitiesDescriptor)MetadataLoader.Load(stream);

                Action a = () => new Federation(metadata, true, Options.FromConfiguration.SPOptions);

                a.ShouldNotThrow();
            }
        }

        [TestMethod]
        public void Federation_LoadSkolfederationMetadata()
        {
            // Skolfederation is the Swedish national school federation. To test that
            // AuthServices handles some real world metadata, the metdata from the
            // skolfederation federation is used.

            TestLoadMetadata("Metadata\\SkolfederationMetadata.xml");
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

        [TestMethod]
        public void Federation_MetadataValidUntil_Loaded()
        {
            var subject = new Federation(
                new Uri("http://localhost:13428/federationMetadata"),
                false,
                StubFactory.CreateSPOptions());

            subject.MetadataValidUntil.Should().Be(new DateTime(2100, 01, 01, 14, 43, 15));
        }

        [TestMethod]
        public void Federation_MetadataValidUntil_CalculatedFromCacheDuration()
        {
            var subject = new Federation(
                new Uri("http://localhost:13428/federationMetadataVeryShortCacheDuration"),
                false,
                StubFactory.CreateSPOptions());

            subject.MetadataValidUntil.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void Federation_ScheduledReloadOfMetadata()
        {
            MetadataRefreshScheduler.minInternval = new TimeSpan(0, 0, 0, 0, 1);

            var subject = new Federation(
                new Uri("http://localhost:13428/federationMetadataVeryShortCacheDuration"),
                false,
                StubFactory.CreateSPOptions());

            var initialValidUntil = subject.MetadataValidUntil;

            SpinWaiter.While(() => subject.MetadataValidUntil == initialValidUntil);
        }
    }
}
