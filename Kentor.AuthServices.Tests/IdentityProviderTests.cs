using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Kentor.AuthServices.TestHelpers;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class IdentityProviderTests
    {
        [TestMethod]
        public void IdentityProvider_CreateAuthenticateRequest_Destination()
        {
            string idpUri = "http://idp.example.com/";
            
            var ip = new IdentityProvider()
            {
                DestinationUri = new Uri(idpUri)
            };

            var r = ip.CreateAuthenticateRequest(null);

            r.ToXElement().Attribute("Destination").Should().NotBeNull()
                .And.Subject.Value.Should().Be(idpUri);
        }

        [TestMethod]
        public void IdentityProvider_CreateAuthenticateRequest_AssertionConsumerServiceUrlFromConfig()
        {
            var idp = IdentityProvider.ConfiguredIdentityProviders.First().Value;

            var r = idp.CreateAuthenticateRequest(null);

            r.AssertionConsumerServiceUrl.Should().Be(new Uri("http://localhost/Saml2AuthenticationModule/acs"));
        }

        [TestMethod]
        public void IdentityProvider_CreateAuthenticateRequest_IssuerFromConfig()
        {
            var idp = IdentityProvider.ConfiguredIdentityProviders.First().Value;

            var r = idp.CreateAuthenticateRequest(null);

            r.Issuer.Should().Be("https://github.com/KentorIT/authservices");
        }

        [TestMethod]
        public void IdentityProvider_Certificate_FromFile()
        {
            var idp = IdentityProvider.ConfiguredIdentityProviders.First().Value;

            idp.Certificate.ShouldBeEquivalentTo(SignedXmlHelper.TestCert);
        }
    }
}
