using FluentAssertions;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.IdentityModel.Metadata;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.WebSso;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class IdentityProviderTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            MetadataServer.IdpVeryShortCacheDurationIncludeInvalidKey = false;
        }

        [TestMethod]
        public void IdentityProvider_CreateAuthenticateRequest_DestinationInXml()
        {
            string idpUri = "http://idp.example.com/";

            var ip = new IdentityProvider(
                new Uri(idpUri),
                Options.FromConfiguration.SPOptions);

            var r = ip.CreateAuthenticateRequest(null, StubFactory.CreateAuthServicesUrls());

            r.ToXElement().Attribute("Destination").Should().NotBeNull()
                .And.Subject.Value.Should().Be(idpUri);
        }

        [TestMethod]
        public void IdentityProvider_CreateAuthenticateRequest_BasicInfo()
        {
            var options = Options.FromConfiguration;

            var idp = options.IdentityProviders.Default;

            var urls = StubFactory.CreateAuthServicesUrls();
            var subject = idp.CreateAuthenticateRequest(null, urls);

            var expected = new Saml2AuthenticationRequest()
            {
                AssertionConsumerServiceUrl = urls.AssertionConsumerServiceUrl,
                DestinationUri = idp.SingleSignOnServiceUrl,
                Issuer = options.SPOptions.EntityId,
                AttributeConsumingServiceIndex = 0,
            };

            subject.ShouldBeEquivalentTo(expected, opt => opt.Excluding(au => au.Id));
        }

        [TestMethod]
        public void IdentityProvider_CreateAuthenticateRequest_NoAttributeIndex()
        {
            var options = StubFactory.CreateOptions();
            var idp = options.IdentityProviders.Default;
            var urls = StubFactory.CreateAuthServicesUrls();

            ((SPOptions)options.SPOptions).AttributeConsumingServices.Clear();

            var subject = idp.CreateAuthenticateRequest(null, urls);

            var expected = new Saml2AuthenticationRequest()
            {
                AssertionConsumerServiceUrl = urls.AssertionConsumerServiceUrl,
                DestinationUri = idp.SingleSignOnServiceUrl,
                Issuer = options.SPOptions.EntityId,
                AttributeConsumingServiceIndex = null
            };

            subject.ShouldBeEquivalentTo(expected, opt => opt.Excluding(au => au.Id));
        }

        [TestMethod]
        public void IdentityProvider_CreateAuthenticateRequest_NullcheckAuthServicesUrls()
        {
            var idp = Options.FromConfiguration.IdentityProviders.Default;

            Action a = () => idp.CreateAuthenticateRequest(new Uri("http://localhost"), null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("authServicesUrls");
        }

        [TestMethod]
        public void IdentityProvider_Certificate_FromFile()
        {
            var idp = Options.FromConfiguration.IdentityProviders.Default;

            idp.SigningKey.ShouldBeEquivalentTo(SignedXmlHelper.TestKey);
        }

        [TestMethod]
        public void IdentityProvider_AllowUnsolicitedAuthnResponse_FromConfig()
        {
            Options.FromConfiguration.IdentityProviders[new EntityId("https://idp.example.com")]
                .AllowUnsolicitedAuthnResponse.Should().BeTrue();

            Options.FromConfiguration.IdentityProviders[new EntityId("https://idp2.example.com")]
                .AllowUnsolicitedAuthnResponse.Should().BeFalse();
        }

        [TestMethod]
        public void IdentityProvider_AllowUnsolicitedAuthnResponse_FromConfigForFederation()
        {
            Options.FromConfiguration.IdentityProviders[new EntityId("http://idp.federation.example.com/metadata")]
                .AllowUnsolicitedAuthnResponse.Should().BeTrue();
        }

        [TestMethod]
        public void IdentityProvider_ConfigFromMetadata()
        {
            var entityId = new EntityId("http://localhost:13428/idpMetadata");
            var idpFromMetadata = Options.FromConfiguration.IdentityProviders[entityId];

            idpFromMetadata.EntityId.Id.Should().Be(entityId.Id);
            idpFromMetadata.Binding.Should().Be(Saml2BindingType.HttpPost);
            idpFromMetadata.SingleSignOnServiceUrl.Should().Be(new Uri("http://localhost:13428/acs"));
            idpFromMetadata.SigningKey.ShouldBeEquivalentTo(SignedXmlHelper.TestKey);
        }

        private IdentityProviderElement CreateConfig()
        {
            var config = new IdentityProviderElement();
            config.AllowConfigEdit(true);
            config.Binding = Saml2BindingType.HttpPost;
            config.SigningCertificate = new CertificateElement();
            config.SigningCertificate.AllowConfigEdit(true);
            config.SigningCertificate.FileName = "Kentor.AuthServices.Tests.pfx";
            config.DestinationUri = new Uri("http://idp.example.com/acs");
            config.EntityId = "http://idp.example.com";

            return config;
        }

        private static void TestMissingConfig(IdentityProviderElement config, string missingElement)
        {
            Action a = () => new IdentityProvider(config, Options.FromConfiguration.SPOptions);

            string expectedMessage = "Missing " + missingElement + " configuration on Idp " + config.EntityId + ".";
            a.ShouldThrow<ConfigurationErrorsException>(expectedMessage);
        }

        [TestMethod]
        public void IdentityProvider_Ctor_MissingBindingThrows()
        {
            var config = CreateConfig();
            config.Binding = 0;
            TestMissingConfig(config, "binding");
        }

        [TestMethod]
        public void IdentityProvider_Ctor_MissingCertificateThrows()
        {
            var config = CreateConfig();

            // Don't set to null; if the section isn't present in the config the
            // loaded configuration will contain an empty SigningCertificate element.
            config.SigningCertificate = new CertificateElement();
            TestMissingConfig(config, "signing certificate");
        }

        [TestMethod]
        public void IdentityProvider_Ctor_MissingDestinationUriThrows()
        {
            var config = CreateConfig();
            config.DestinationUri = null;
            TestMissingConfig(config, "assertion consumer service url");
        }

        [TestMethod]
        public void IdentityProvider_Ctor_HandlesConfiguredCertificateWhenReadingMetadata()
        {
            var config = CreateConfig();
            config.LoadMetadata = true;
            config.EntityId = "http://localhost:13428/idpMetadataNoCertificate";

            var subject = new IdentityProvider(config, Options.FromConfiguration.SPOptions);

            // Check that metadata was read and overrides configured values.
            subject.Binding.Should().Be(Saml2BindingType.HttpRedirect);
            subject.SigningKey.ShouldBeEquivalentTo(SignedXmlHelper.TestKey);
        }

        [TestMethod]
        public void IdentityProvider_Ctor_WrongEntityIdInMetadata()
        {
            var config = CreateConfig();
            config.LoadMetadata = true;
            config.EntityId = "http://localhost:13428/idpMetadataWrongEntityId";

            Action a = () => new IdentityProvider(config, Options.FromConfiguration.SPOptions);

            a.ShouldThrow<ConfigurationErrorsException>().And.Message.Should()
                .Be("Unexpected entity id \"http://wrong.entityid.example.com\" found when loading metadata for \"http://localhost:13428/idpMetadataWrongEntityId\".");
        }

        [TestMethod]
        public void IdentityProvider_Ctor_PrefersRedirectBindingFromMetadata()
        {
            var config = CreateConfig();
            config.LoadMetadata = true;
            config.EntityId = "http://localhost:13428/idpMetadataWithMultipleBindings";

            var subject = new IdentityProvider(config, Options.FromConfiguration.SPOptions);

            subject.Binding.Should().Be(Saml2BindingType.HttpRedirect);
            subject.SingleSignOnServiceUrl.Should().Be("http://idp2Bindings.example.com/Redirect");
        }

        [TestMethod]
        public void IdentityProvider_Ctor_ChoosesSupportedBindingFromMetadata()
        {
            var config = CreateConfig();
            config.LoadMetadata = true;
            config.EntityId = "http://localhost:13428/idpMetadataWithArtifactBinding";

            var subject = new IdentityProvider(config, Options.FromConfiguration.SPOptions);

            subject.Binding.Should().Be(Saml2BindingType.HttpPost);
            subject.SingleSignOnServiceUrl.Should().Be("http://idpArtifact.example.com/POST");
        }

        [TestMethod]
        public void IdentityProvider_Ctor_UseMetadataLocationUrl()
        {
            var config = CreateConfig();
            config.LoadMetadata = true;
            config.MetadataUrl = new Uri("http://localhost:13428/idpMetadataDifferentEntityId");
            config.EntityId = "some-idp";

            var subject = new IdentityProvider(config, Options.FromConfiguration.SPOptions);

            subject.Binding.Should().Be(Saml2BindingType.HttpRedirect);
            subject.SingleSignOnServiceUrl.Should().Be("http://idp.example.com/SsoService");
        }

        [TestMethod]
        public void IdentityProvider_MetadataValidUntil_NullOnConfigured()
        {
            string idpUri = "http://idp.example.com/";

            var subject = new IdentityProvider(
                new Uri(idpUri),
                Options.FromConfiguration.SPOptions);

            subject.MetadataValidUntil.Should().NotHaveValue();
        }

        [TestMethod]
        public void IdentityProvider_MetadataValidUntil_Loaded()
        {
            var config = CreateConfig();
            config.LoadMetadata = true;
            config.EntityId = "http://localhost:13428/idpMetadata";

            var subject = new IdentityProvider(config, Options.FromConfiguration.SPOptions);

            subject.MetadataValidUntil.Should().Be(new DateTime(2100, 01, 02, 14, 42, 43, DateTimeKind.Utc));
        }

        [TestMethod]
        public void IdentityProvider_MetadataValidUntil_CalculatedFromCacheDuration()
        {
            var config = CreateConfig();
            config.LoadMetadata = true;
            config.EntityId = "http://localhost:13428/idpMetadataNoCertificate";

            var subject = new IdentityProvider(config, Options.FromConfiguration.SPOptions);

            var expectedValidUntil = DateTime.UtcNow.AddMinutes(15);
            subject.MetadataValidUntil.Should().BeCloseTo(expectedValidUntil);
        }

        TimeSpan maxWait = new TimeSpan(0, 0, 0, 0, 20);
        private void WaitForMetadataReload(Func<bool> condition)
        {
            var waitStart = DateTime.UtcNow;
            while (condition())
            {
                if (DateTime.UtcNow - waitStart > maxWait)
                {
                    Assert.Fail("Timeout passed without metadata being refreshed");
                }
            }
        }

        IdentityProvider CreateSubjectForMetadataRefresh()
        {
            var config = CreateConfig();
            config.LoadMetadata = true;
            config.EntityId = "http://localhost:13428/idpMetadataVeryShortCacheDuration";
            return new IdentityProvider(config, Options.FromConfiguration.SPOptions);
        }

        [TestMethod]
        public void IdentityProvider_MetadataValidUntil_ReloadsMetadataIfNoLongerValid()
        {
            var subject = CreateSubjectForMetadataRefresh();
            var initialValidUntil = subject.MetadataValidUntil;
            WaitForMetadataReload(() => subject.MetadataValidUntil == initialValidUntil);
        }

        [TestMethod]
        public void IdentityProvider_Binding_ReloadsMetadataIfNoLongerValid()
        {
            MetadataServer.IdpVeryShortCacheDurationBinding = Saml2Binding.HttpRedirectUri;
            var subject = CreateSubjectForMetadataRefresh();
            subject.Binding.Should().Be(Saml2BindingType.HttpRedirect);
            MetadataServer.IdpVeryShortCacheDurationBinding = Saml2Binding.HttpPostUri;

            WaitForMetadataReload(() => subject.Binding == Saml2BindingType.HttpRedirect);

            subject.Binding.Should().Be(Saml2BindingType.HttpPost);
        }

        [TestMethod]
        public void IdentityProvider_SingleSignOnServiceUrl_ReloadsMetadataIfNoLongerValid()
        {
            MetadataServer.IdpVeryShortCacheDurationSsoPort = 42;
            var subject = CreateSubjectForMetadataRefresh();
            subject.SingleSignOnServiceUrl.Port.Should().Be(42);
            MetadataServer.IdpVeryShortCacheDurationSsoPort = 117;

            WaitForMetadataReload(() => subject.SingleSignOnServiceUrl.Port == 42);

            subject.SingleSignOnServiceUrl.Port.Should().Be(117);
        }

        [TestMethod]
        public void IdentityProvider_SigningKey_ReloadsMetadataIfNoLongerValid()
        {
            var subject = CreateSubjectForMetadataRefresh();
            subject.SigningKey.Should().NotBeNull();
            MetadataServer.IdpVeryShortCacheDurationIncludeInvalidKey = true;

            Action a = () => WaitForMetadataReload(() => subject.SigningKey == null);

            a.ShouldThrow<System.Xml.XmlException>();
        }
    }
}
