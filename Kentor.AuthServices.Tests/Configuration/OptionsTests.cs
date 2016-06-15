using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.IdentityModel.Metadata;
using System.Collections.Generic;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.WebSso;
using System.IdentityModel.Tokens;
using Kentor.AuthServices.Tests.Helpers;
using System.Linq;
using System.Security.Claims;

namespace Kentor.AuthServices.Tests.Configuration
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

            a.ShouldThrow<KeyNotFoundException>().And.Message.Should().Be("No Idp with entity id \"urn:Non.Existent.EntityId\" found.");
        }

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
                .ShouldNotThrow();
        }
    }
}
