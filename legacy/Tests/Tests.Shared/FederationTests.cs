using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using FluentAssertions;
using System.Linq;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Tokens;
using Sustainsys.Saml2.Tests.Helpers;
using Sustainsys.Saml2.Tests.Metadata;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using Sustainsys.Saml2.Exceptions;
using System.Security.Cryptography.Xml;
using Sustainsys.Saml2.Metadata.Exceptions;
using Sustainsys.Saml2.Metadata.Tokens;
using Sustainsys.Saml2.TestHelpers;

namespace Sustainsys.Saml2.Tests
{
    [TestClass]
    public class FederationTests
    {
        TimeSpan refreshMinInterval = MetadataRefreshScheduler.minInterval;

        [TestCleanup]
        public void Cleanup()
        {
            StubServer.IdpVeryShortCacheDurationIncludeInvalidKey = false;
            StubServer.FederationVeryShortCacheDurationSecondAlternativeEnabled = false;
            StubServer.IdpAndFederationShortCacheDurationAvailable = true;
            MetadataRefreshScheduler.minInterval = refreshMinInterval;
        }

        [TestMethod]
        public void Federation_Ctor_NullcheckConfig()
        {
            Action a = () => new Federation(null, Options.FromConfiguration);

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("config");
        }

        [TestMethod]
        public void Federation_Ctor_LoadsConfig()
        {
            var config = SustainsysSaml2Section.Current
                .Federations.First();

            var options = StubFactory.CreateOptions();

            var subject = new Federation(config, options);

            subject.metadataLocation.Should().Be("http://localhost:13428/federationMetadataSigned");
            subject.allowUnsolicitedAuthnResponse.Should().BeTrue();
            subject.SigningKeys.First().As<X509RawDataKeyIdentifierClause>()
                .Matches(SignedXmlHelper.TestCert).Should().BeTrue();
        }

        [TestMethod]
        public void Federation_Ctor_ConvertsEmptySigningCertificateFromConfigToNull()
        {
            var config = SustainsysSaml2Section.Current
                .Federations.Skip(1).Single();

            var options = StubFactory.CreateOptions();

            var subject = new Federation(config, options);

            subject.SigningKeys.Should().BeNull();
        }

        [TestMethod]
        public void Federation_LoadSambiTestMetadata()
        {
            // Sambi is the Swedish health care federation. To test that Saml2
            // handles some real world metadata, the metadadata from Sambi's test
            // environment is used.

            var options = StubFactory.CreateOptions();

            var path = "~/Metadata/SambiMetadata.xml";
            var idpInFederation = new EntityId("http://idp-acc.test.ek.sll.se/neas");

            Action a = () => new Federation(path, true, options);

            a.Should().NotThrow();

            IdentityProvider idp;
            options.IdentityProviders.TryGetValue(idpInFederation, out idp)
                .Should().BeTrue();
        }

        [TestMethod]
        public void Federation_LoadSkolfederationMetadata()
        {
            // Skolfederation is the Swedish national school federation. To test that
            // Saml2 handles some real world metadata, the metadata from the
            // skolfederation federation is used.

            var options = StubFactory.CreateOptions();

            var path = "~/Metadata/SkolfederationMetadata.xml";
            var idpInFederation = new EntityId("http://fs.ale.se/adfs/services/trust");

            Action a = () => new Federation(path, true, options);

            a.Should().NotThrow();

            IdentityProvider idp;
            options.IdentityProviders.TryGetValue(idpInFederation, out idp)
                .Should().BeTrue();
        }

        [TestMethod]
        public void Federation_LoadInCommonMetadata()
        {
            // InCommon is the large US university federation. To test that
            // Saml2 handles some real world metadata, the metadata from
            // the InCommon federation is used.

            var options = StubFactory.CreateOptions();

            var path = "~/Metadata/InCommonMetadata.xml";
            var idpInFederation = new EntityId("https://shibboleth.umassmed.edu/idp/shibboleth");

            Action a = () => new Federation(path, true, options);

            a.Should().NotThrow();

            IdentityProvider idp;
            options.IdentityProviders.TryGetValue(idpInFederation, out idp)
                .Should().BeTrue();
        }

        [TestMethod]
        public void Federation_Ctor_MetadataLocation()
        {
            var options = StubFactory.CreateOptions();

            var subject = new Federation(
                "http://localhost:13428/federationMetadata",
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
                "http://localhost:13428/federationMetadata",
                false,
                StubFactory.CreateOptions());

            subject.MetadataValidUntil.Should().Be(new DateTime(2100, 01, 01, 14, 43, 15));
        }

        [TestMethod]
        public void Federation_MetadataValidUntil_CalculatedFromCacheDuration()
        {
            var subject = new Federation(
                "http://localhost:13428/federationMetadataVeryShortCacheDuration",
                false,
                StubFactory.CreateOptions());

            subject.MetadataValidUntil.Should().BeCloseTo(DateTime.UtcNow);
        }

        [TestMethod]
        public void Federation_ScheduledReloadOfMetadata()
        {
            MetadataRefreshScheduler.minInterval = new TimeSpan(0, 0, 0, 0, 1);

            var subject = new Federation(
                "http://localhost:13428/federationMetadataVeryShortCacheDuration",
                false,
                StubFactory.CreateOptions());

            var initialValidUntil = subject.MetadataValidUntil;

            SpinWaiter.WhileEqual(() => subject.MetadataValidUntil, () => initialValidUntil);
        }

        [TestMethod]
        public void Federation_ReloadOfMetadata_AddsNewIdpAndRemovesOld()
        {
            MetadataRefreshScheduler.minInterval = new TimeSpan(0, 0, 0, 0, 1);

            var options = StubFactory.CreateOptions();

            var subject = new Federation(
                "http://localhost:13428/federationMetadataVeryShortCacheDuration",
                false,
                options);

            IdentityProvider idp;
            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeTrue("idp1 should be loaded initially");
            options.IdentityProviders.TryGetValue(new EntityId("http://idp2.federation.example.com/metadata"), out idp)
                .Should().BeTrue("idp2 should be loaded initially");
            options.IdentityProviders.TryGetValue(new EntityId("http://idp3.federation.example.com/metadata"), out idp)
                .Should().BeFalse("idp3 shouldn't be loaded initially");

            StubServer.FederationVeryShortCacheDurationSecondAlternativeEnabled = true;
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
            MetadataRefreshScheduler.minInterval = new TimeSpan(0, 0, 0, 0, 1);

            StubServer.IdpAndFederationShortCacheDurationAvailable = false;

            var options = StubFactory.CreateOptions();

            var subject = new Federation(
                "http://localhost:13428/federationMetadataVeryShortCacheDuration",
                false,
                options);

            subject.MetadataValidUntil.Should().Be(DateTime.MinValue);

            StubServer.IdpAndFederationShortCacheDurationAvailable = true;

            SpinWaiter.WhileEqual(() => subject.MetadataValidUntil, () => DateTime.MinValue);

            IdentityProvider idp;
            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeTrue();
        }

        [TestMethod]
        public void Federation_ReloadOfMetadata_RemovesAllIdpsIfMetadataIsNoLongerValid()
        {
            MetadataRefreshScheduler.minInterval = new TimeSpan(0, 0, 0, 0, 1);

            var options = StubFactory.CreateOptions();

            var subject = new Federation(
                "http://localhost:13428/federationMetadataVeryShortCacheDuration",
                false,
                options);

            IdentityProvider idp;
            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeTrue();

            StubServer.IdpAndFederationShortCacheDurationAvailable = false;

            SpinWaiter.WhileNotEqual(() => subject.MetadataValidUntil, () => DateTime.MinValue);

            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeFalse("idp should be removed if metadata is no longer valid");
        }

        [TestMethod]
        public void Federation_ReloadOfMetadata_KeepsOldDataUntilMetadataBecomesInvalid()
        {
            MetadataRefreshScheduler.minInterval = new TimeSpan(0, 0, 0, 0, 5);

            var options = StubFactory.CreateOptions();
            // Disable logging in this case, to trigger the code path when logger is null.
            options.SPOptions.Logger = null;

            var subject = new Federation(
                "http://localhost:13428/federationMetadataShortCacheDuration",
                false,
                options);

            IdentityProvider idp;
            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeTrue("idp should be loaded initially");

            var initialValidUntil = subject.MetadataValidUntil;

            StubServer.IdpAndFederationShortCacheDurationAvailable = false;

            try
            {
                // Wait until a failed load has occured.
                SpinWaiter.While(() => subject.LastMetadataLoadException == null,
                    "Timeout passed without a failed metadata reload.");

                subject.MetadataValidUntil.Should().NotBe(DateTime.MinValue);
            }
            catch(AssertFailedException)
            {
                Assert.Inconclusive("This test is sensitive to race conditions, didn't work this time. Don't worry.");
            }

            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeTrue("idp shouldn't be removed while metadata is still valid.");

            SpinWaiter.While(() => subject.MetadataValidUntil != DateTime.MinValue,
                "Timeout passed without metadata becoming invalid.");

            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeFalse("idp should be removed if metadata is no longer valid");

            StubServer.IdpAndFederationShortCacheDurationAvailable = true;

            SpinWaiter.While(() => subject.MetadataValidUntil == DateTime.MinValue,
                "Timeout passed without metadata being successfully reloaded");

            options.IdentityProviders.TryGetValue(new EntityId("http://idp1.federation.example.com/metadata"), out idp)
                .Should().BeTrue("idp should be readded when metadata is refreshed.");
        }

        [TestMethod]
        public void Federation_RejectsIfNotSignedWhenConfiguredWithKeys()
        {
            var options = StubFactory.CreateOptions();

            var metadataLocation = "http://localhost:13428/federationMetadata";

            var subject = new Federation(
                metadataLocation,
                true,
                options,
                new List<X509Certificate2>()
                {
                    SignedXmlHelper.TestCert
                });

            subject.LastMetadataLoadException.As<InvalidSignatureException>()
                .Message.Should().Match("Signature*failed*");
        }

        [TestMethod]
        public void Federation_AcceptsCorrectlySignedMetadataWhenConfiguredWithKeys()
        {
            var options = StubFactory.CreateOptions();

            var metadataLocation = "http://localhost:13428/federationMetadataSigned";

            var subject = new Federation(
                metadataLocation,
                true,
                options,
                new List<X509Certificate2>()
                {
                    SignedXmlHelper.TestCert
                });

            subject.LastMetadataLoadException.Should().BeNull();
        }

        [TestMethod]
        public void Federation_RejectsTamperedMetadataWhenConfiguredWithKeys()
        {
            var options = StubFactory.CreateOptions();

            var metadataLocation = "http://localhost:13428/federationMetadataSignedTampered";

            var subject = new Federation(
                metadataLocation,
                true,
                options,
                new List<X509Certificate2>()
                {
                    SignedXmlHelper.TestCert
                });

            subject.LastMetadataLoadException.As<InvalidSignatureException>()
                .Message.Should().Match("*tampered*");
        }

        [TestMethod]
        public void Federation_RejectsTooWeakMetadataSignatureAlgorithmWhenConfiguredWithKeys()
        {
            var options = StubFactory.CreateOptions();
            options.SPOptions.MinIncomingSigningAlgorithm = SecurityAlgorithms.RsaSha512Signature;

            var metadataLocation = "http://localhost:13428/federationMetadataSigned";

            var subject = new Federation(
                metadataLocation,
                true,
                options,
                new List<X509Certificate2>()
                {
                    SignedXmlHelper.TestCert
                });

            subject.LastMetadataLoadException.As<InvalidSignatureException>()
                .Message.Should().Match("*algorithm*256*weak*512*");
        }

        [TestMethod]
        public void Federation_ValidatesCertificateWhenConfigured()
        {
            var options = StubFactory.CreateOptions();
            options.SPOptions.ValidateCertificates = true;

            var metadataLocation = "http://localhost:13428/federationMetadataSigned";

            var subject = new Federation(
                metadataLocation,
                true,
                options,
                new List<X509Certificate2>()
                {
                    SignedXmlHelper.TestCert
                });

            subject.LastMetadataLoadException.As<InvalidSignatureException>()
                .Message.Should().Match("*verification*certificate*failed*");
        }
    }
}
