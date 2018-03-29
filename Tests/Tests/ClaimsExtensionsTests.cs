using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens.Saml2;

using ClaimProperties = Microsoft.IdentityModel.Tokens.Saml2.ClaimProperties;

namespace Sustainsys.Saml2.Tests
{
    [TestClass]
    public class ClaimsExtensionsTests
    {
        [TestMethod]
        public void ClaimsExtensions_ToSaml2NameIdentifier_NullCheck()
        {
			Action a = () => ((Claim)null).ToSaml2NameIdentifier();
            a.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("claim");
        }

        [TestMethod]
        public void ClaimsExtensions_ToSaml2NameIdentifier_CheckClaimType()
        {
            Action a = () => new Claim(ClaimTypes.CookiePath, "foo")
            .ToSaml2NameIdentifier();

            a.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("claim");
        }

        [TestMethod]
        public void ClaimsExtensions_ToSaml2NameIdentifier_LogoutNameIdentifier_NameOnly()
        {
            var claim = new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,,,NameId");

            var actual = claim.ToSaml2NameIdentifier();

            var expected = new Saml2NameIdentifier("NameId");

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ClaimsExtensions_ToSaml2NameIdentifier_LogoutNameIdentifier_NameIdFormat()
        {
            var claim = new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,urn:foo,,NameId");

            var actual = claim.ToSaml2NameIdentifier();

            var expected = new Saml2NameIdentifier("NameId")
            {
                Format = new Uri("urn:foo")
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ClaimsExtensions_ToSaml2NameIdentifier_LogoutNameIdentifier_NameQualifier()
        {
            var claim = new Claim(Saml2ClaimTypes.LogoutNameIdentifier, "qualifier,,,,NameId");

            var actual = claim.ToSaml2NameIdentifier();

            var expected = new Saml2NameIdentifier("NameId")
            {
                NameQualifier = "qualifier"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ClaimsExtensions_ToSaml2NameIdentifier_LogoutNameIdentifier_SPNameQualifier()
        {
            var claim = new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",qualifier,,,NameId");

            var actual = claim.ToSaml2NameIdentifier();

            var expected = new Saml2NameIdentifier("NameId")
            {
                SPNameQualifier = "qualifier"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ClaimsExtensions_ToSaml2NameIdentifier_LogoutNameIdentifier_SPProvidedId()
        {
            var claim = new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,,spId,NameId");

            var actual = claim.ToSaml2NameIdentifier();

            var expected = new Saml2NameIdentifier("NameId")
            {
                SPProvidedId = "spId"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ClaimsExtensions_ToSaml2NameIdentifier_NameIdentifier_NameOnly()
        {
            var claim = new Claim(ClaimTypes.NameIdentifier, "NameId");

            var actual = claim.ToSaml2NameIdentifier();

            var expected = new Saml2NameIdentifier("NameId");

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ClaimsExtensions_ToSaml2NameIdentifier_NameIdentifier_NameIdFormat()
        {
            var claim = new Claim(ClaimTypes.NameIdentifier, "NameId");
            claim.Properties[ClaimProperties.SamlNameIdentifierFormat] = "urn:foo";

            var actual = claim.ToSaml2NameIdentifier();

            var expected = new Saml2NameIdentifier("NameId")
            {
                Format = new Uri("urn:foo")
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ClaimsExtensions_ToSaml2NameIdentifier_NameIdentifier_NameQualifier()
        {
            var claim = new Claim(ClaimTypes.NameIdentifier, "NameId");
            claim.Properties[ClaimProperties.SamlNameIdentifierNameQualifier] = "qualifier";

            var actual = claim.ToSaml2NameIdentifier();

            var expected = new Saml2NameIdentifier("NameId")
            {
                NameQualifier = "qualifier"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ClaimsExtensions_ToSaml2NameIdentifier_NameIdentifier_SPNameQualifier()
        {
            var claim = new Claim(ClaimTypes.NameIdentifier, "NameId");
            claim.Properties[ClaimProperties.SamlNameIdentifierSPNameQualifier] = "qualifier";

            var actual = claim.ToSaml2NameIdentifier();

            var expected = new Saml2NameIdentifier("NameId")
            {
                SPNameQualifier = "qualifier"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ClaimsExtensions_ToSaml2NameIdentifier_NameIdentifier_SPProvidedId()
        {
            var claim = new Claim(ClaimTypes.NameIdentifier, "NameId");
            claim.Properties[ClaimProperties.SamlNameIdentifierSPProvidedId] = "spId";

            var actual = claim.ToSaml2NameIdentifier();

            var expected = new Saml2NameIdentifier("NameId")
            {
                SPProvidedId = "spId"
            };

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
