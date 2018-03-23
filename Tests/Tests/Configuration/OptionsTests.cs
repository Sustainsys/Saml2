using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Sustainsys.Saml2.Metadata;
using System.Collections.Generic;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.WebSso;
using Sustainsys.Saml2.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using Sustainsys.Saml2.TestHelpers;

namespace Sustainsys.Saml2.Tests.Configuration
{
    [TestClass]
    public class OptionsTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            SignedXmlHelper.RemoveGlobalSha256XmlSignatureSupport();
        }

        [TestMethod]
        public void Options_FromConfiguration_IdentityProviders_IncludeIdpFromFederation()
        {
            var subject = Options.FromConfiguration.IdentityProviders[
                new EntityId("http://idp.federation.example.com/metadata")];

            subject.EntityId.Id.Should().Be("http://idp.federation.example.com/metadata");
            subject.Binding.Should().Be(Saml2BindingType.HttpRedirect);
        }

        [TestMethod]
        public void Options_FromConfiguration_IdentityProviders_ThrowsOnInvalidEntityId()
        {
            Action a = () =>
            {
                var i = Options.FromConfiguration.IdentityProviders[
                new EntityId("urn:Non.Existent.EntityId")];
            };

            a.Should().Throw<KeyNotFoundException>().And.Message.Should().Be("No Idp with entity id \"urn:Non.Existent.EntityId\" found.");
        }

		#if FALSE
        [TestMethod]
        public void Options_GlobalEnableSha256Signatures_DoesntBreakJwtSecurityTokenHandler()
        {
            // The JWT security token handler that is used by IdentityServer3
            // uses the global CryptoConfig. This has been causing issues
            // when registering the Sha256 algorithm globally, where this
            // alters the result of the JWT signature handler lookup.

            Options.GlobalEnableSha256XmlSignatures();

            var cert = SignedXmlHelper.TestCert;

            var token = new JwtSecurityToken(
                new JwtHeader(new X509SigningCredentials(SignedXmlHelper.TestCert2)),
                new JwtPayload(Enumerable.Empty<Claim>()));

            var handler = new JwtSecurityTokenHandler();

            handler.Invoking(h => h.WriteToken(token))
                .Should().NotThrow();
        }
		#endif

        [TestMethod]
        public void Options_GlobalEnableSha256Signatures_DoesntAlterKnownAlgorithmsIfSha256AlreadyPresent()
        {
            var knownAlgorithmsCopy = XmlHelpers.KnownSigningAlgorithms.ToList();

            Options.AddAlgorithmIfMissing(knownAlgorithmsCopy, SecurityAlgorithms.RsaSha256Signature);

            knownAlgorithmsCopy.Should().BeEquivalentTo(XmlHelpers.KnownSigningAlgorithms);
        }

        [TestMethod]
        public void Options_GlobalEnableSha256Signatures_AddsSha256IfOnlySha1InList()
        {
            var knownAlgorithms = new List<string>()
            {
                SignedXml.XmlDsigRSASHA1Url
            };

            Options.AddAlgorithmIfMissing(knownAlgorithms, SecurityAlgorithms.RsaSha256Signature);

            var expected = new List<string>()
            {
                SignedXml.XmlDsigRSASHA1Url,
                SecurityAlgorithms.RsaSha256Signature
            };

            knownAlgorithms.Should().BeEquivalentTo(expected);
        }
    }
}
