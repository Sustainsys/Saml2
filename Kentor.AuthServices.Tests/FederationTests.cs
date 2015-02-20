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
using System.Threading;

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
            MetadataServer.FederationVeryShortCacheDurationSecondAlternativeEnabled = false;
            MetadataServer.IdpAndFederationShortCacheDurationAvailable = true;
            MetadataRefreshScheduler.minInternval = refreshMinInterval;
        }

        [TestMethod]
        public void Federation_Ctor_NullcheckConfig()
        {
            Action a = () => new Federation(null, Options.FromConfiguration);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("config");
        }

        [TestMethod]
        public void Federation_LoadSambiTestMetadata()
        {
            // Sambi is the Swedish health care federation. To test that AuthServices
            // handles some real world metadata, the metadadata from Sambi's test
            // environment is used.

            var options = StubFactory.CreateOptions();

            var url = new Uri("http://localhost:13428/SambiMetadata");
            var idpInFederation = new EntityId("http://idp-acc.test.ek.sll.se/neas");

            Action a = () => new Federation(url, true, options);

            a.ShouldNotThrow();

            IdentityProvider idp;
            options.IdentityProviders.TryGetValue(idpInFederation, out idp)
                .Should().BeTrue();
        }

        [TestMethod]
        public void Federation_LoadSkolfederationMetadata()
        {
            // Skolfederation is the Swedish national school federation. To test that
            // AuthServices handles some real world metadata, the metdata from the
            // skolfederation federation is used.

            var options = StubFactory.CreateOptions();

            var url = new Uri("http://localhost:13428/SkolfederationMetadata");
            var idpInFederation = new EntityId("http://fs.ale.se/adfs/services/trust");

            Action a = () => new Federation(url, true, options);

            a.ShouldNotThrow();

            IdentityProvider idp;
            options.IdentityProviders.TryGetValue(idpInFederation, out idp)
                .Should().BeTrue();
        }

        [TestMethod]
        public void Federation_Ctor_MetadataUrl()
        {
            var options = StubFactory.CreateOptions();

            var subject = new Federation(
                new Uri("http://localhost:13428/federationMetadata"),
                false,
                options);

            IdentityProvider idp;
            options.IdentityProviders
                .TryGetValue(new EntityId("http://idp.federation.example.com/metadata"), out idp)
                .Should().BeTrue();
        }

        [TestMethod]
        public void Federation_MetadataValidUntil_Loaded()
        {
            var subject = new Federation(
                new Uri("http://localhost:13428/federationMetadata"),
                false,
                StubFactory.CreateOptions());

            subject.MetadataValidUntil.Should().Be(new DateTime(2100, 01, 01, 14, 43, 15));
        }

        [TestMethod]
        public void Federation_MetadataValidUntil_CalculatedFromCacheDuration()
        {
            var subject = new Federation(
                new Uri("http://localhost:13428/federationMetadataVeryShortCacheDuration"),
                false,
                StubFactory.CreateOptions());

            subject.MetadataValidUntil.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void Federation_ScheduledReloadOfMetadata()
        {
            MetadataRefreshScheduler.minInternval = new TimeSpan(0, 0, 0, 0, 1);

            var subject = new Federation(
                new Uri("http://localhost:13428/federationMetadataVeryShortCacheDuration"),
                false,
                StubFactory.CreateOptions());

            var initialValidUntil = subject.MetadataValidUntil;

            SpinWaiter.WhileEqual(() => subject.MetadataValidUntil, () => initialValidUntil);
        }

        [TestMethod]
        public void Federation_ReloadOfMetadata_AddsNewIdpAndRemovesOld()
        {
            MetadataRefreshScheduler.minInternval = new TimeSpan(0, 0, 0, 0, 1);

            var options = StubFactory.CreateOptions();

            var subject = new Federation(
                new Uri("http://localhost:13428/federationMetadataVeryShortCacheDuration"),
                false,
                options);

            IdentityProvider idp;
            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeTrue("idp1 should be loaded initially");
            options.IdentityProviders.TryGetValue(new EntityId("http://idp2.federation.example.com/metadata"), out idp)
                .Should().BeTrue("idp2 should be loaded initially");
            options.IdentityProviders.TryGetValue(new EntityId("http://idp3.federation.example.com/metadata"), out idp)
                .Should().BeFalse("idp3 shouldn't be loaded initially");

            MetadataServer.FederationVeryShortCacheDurationSecondAlternativeEnabled = true;
            var initialValidUntil = subject.MetadataValidUntil;
            SpinWaiter.WhileEqual(() => subject.MetadataValidUntil, () => initialValidUntil);

            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeTrue("idp1 should still be present after reload");
            options.IdentityProviders.TryGetValue(new EntityId("http://idp2.federation.example.com/metadata"), out idp)
                .Should().BeFalse("idp2 should be removed after reload");
            options.IdentityProviders.TryGetValue(new EntityId("http://idp3.federation.example.com/metadata"), out idp)
                .Should().BeTrue("idp3 should be loaded after reload");
        }

        [TestMethod]
        public void Federation_ReloadOfMetadata_RetriesAfterFailedInitialLoad()
        {
            MetadataRefreshScheduler.minInternval = new TimeSpan(0, 0, 0, 0, 1);

            MetadataServer.IdpAndFederationShortCacheDurationAvailable = false;

            var options = StubFactory.CreateOptions();

            var subject = new Federation(
                new Uri("http://localhost:13428/federationMetadataVeryShortCacheDuration"),
                false,
                options);

            subject.MetadataValidUntil.Should().Be(DateTime.MinValue);

            MetadataServer.IdpAndFederationShortCacheDurationAvailable = true;

            SpinWaiter.WhileEqual(() => subject.MetadataValidUntil, () => DateTime.MinValue);

            IdentityProvider idp;
            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeTrue();
        }

        [TestMethod]
        public void Federation_ReloadOfMetadata_RemovesAllIdpsIfMetadataIsNoLongerValid()
        {
            MetadataRefreshScheduler.minInternval = new TimeSpan(0, 0, 0, 0, 1);

            var options = StubFactory.CreateOptions();

            var subject = new Federation(
                new Uri("http://localhost:13428/federationMetadataVeryShortCacheDuration"),
                false,
                options);

            IdentityProvider idp;
            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeTrue();

            MetadataServer.IdpAndFederationShortCacheDurationAvailable = false;

            SpinWaiter.WhileNotEqual(() => subject.MetadataValidUntil, () => DateTime.MinValue);

            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeFalse("idp should be removed if metadata is no longer valid");
        }

        [TestMethod]
        public void Federation_ReloadOfMetadata_KeepsOldDataUntilMetadataBecomesInvalid()
        {
            MetadataRefreshScheduler.minInternval = new TimeSpan(0, 0, 0, 0, 5);

            var options = StubFactory.CreateOptions();

            var subject = new Federation(
                new Uri("http://localhost:13428/federationMetadataShortCacheDuration"),
                false,
                options);

            IdentityProvider idp;
            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeTrue("idp should be loaded initially");

            var initialValidUntil = subject.MetadataValidUntil;

            MetadataServer.IdpAndFederationShortCacheDurationAvailable = false;

            // Wait until a failed load has occured.
            SpinWaiter.While(() => subject.LastMetadataLoadException == null,
                "Timeout passed without a failed metadata reload.");

            subject.MetadataValidUntil.Should().NotBe(DateTime.MinValue);

            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeTrue("idp shouldn't be removed while metadata is still valid.");

            SpinWaiter.While(() => subject.MetadataValidUntil != DateTime.MinValue,
                "Timeout passed without metadata becoming invalid.");

            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeFalse("idp should be removed if metadata is no longer valid");

            MetadataServer.IdpAndFederationShortCacheDurationAvailable = true;

            SpinWaiter.While(() => subject.MetadataValidUntil == DateTime.MinValue,
                "Timeout passed without metadata being successfully reloaded");

            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeTrue("idp should be readded when metadata is refreshed.");
        }
    }
}
