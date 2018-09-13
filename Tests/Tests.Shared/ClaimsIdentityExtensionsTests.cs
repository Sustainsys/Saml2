using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using FluentAssertions;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace Sustainsys.Saml2.Tests
{
    [TestClass]
    public class ClaimsIdentityExtensionsTests
    {
        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_ThrowsOnNullIdentity()
        {
            ClaimsIdentity identity = null;

            Action a = () => identity.ToSaml2Assertion(new EntityId("foo"));

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("identity");
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_ThrowsOnNullIssuer()
        {
            var subject = new ClaimsIdentity();

            Action a = () => subject.ToSaml2Assertion(null);

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("issuer");
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_Includes_Subject()
        {
            var subject = "JohnDoe";

            var ci = new ClaimsIdentity(new Claim[] { 
                new Claim(ClaimTypes.NameIdentifier, subject)
            });

            var a = ci.ToSaml2Assertion(new EntityId("http://idp.example.com"));

            a.Subject.NameId.Value.Should().Be(subject);
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_Sets_SessionIndex()
        {
            var subject = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "NameId"),
                new Claim(Saml2ClaimTypes.SessionIndex, "SessionID"),
                new Claim(ClaimTypes.Email, "me@example.com")
            });

            var issuer = new EntityId("http://idp.example.com");
            var actual = subject.ToSaml2Assertion(issuer);

            actual.Statements.OfType<Saml2AuthenticationStatement>()
                .Single().SessionIndex.Should().Be("SessionID");

            var attributes = actual.Statements.OfType<Saml2AttributeStatement>()
                .Single().Attributes;

            attributes.Should().HaveCount(1);

            attributes.Single().Should().BeEquivalentTo(
                new Saml2Attribute(ClaimTypes.Email, "me@example.com"));
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_Includes_Attributes()
        {
            var ci = new ClaimsIdentity(new Claim[] { 
                new Claim(ClaimTypes.NameIdentifier, "JohnDoe"),
                new Claim(ClaimTypes.Role, "Test"),
                new Claim(ClaimTypes.Email, "me@example.com"),
            });

            var actual = ci.ToSaml2Assertion(new EntityId("http://idp.example.com"));

            actual.Statements.OfType<Saml2AttributeStatement>().Should().HaveCount(1);
            actual.Statements.OfType<Saml2AttributeStatement>().Single().Attributes.First().Values.First().Should().Be("Test");
            actual.Statements.OfType<Saml2AttributeStatement>().Single().Attributes.ElementAt(1).Values.First().Should().Be("me@example.com");
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_MultipleValuesForSameKey_CombinesTo_OneAttribute()
        {
            var ci = new ClaimsIdentity(new Claim[] { 
                new Claim(ClaimTypes.NameIdentifier, "JohnDoe"),
                new Claim(ClaimTypes.Role, "Test1"),
                new Claim(ClaimTypes.Role, "Test2"),
            });

            var assertion = ci.ToSaml2Assertion(new EntityId("http://idp.example.com"));

            assertion.Statements.OfType<Saml2AttributeStatement>().Should().HaveCount(1);
            var actual = assertion.Statements.OfType<Saml2AttributeStatement>().Single();

            var expected = new Saml2AttributeStatement(
                new Saml2Attribute(ClaimTypes.Role, new[] { "Test1", "Test2" }));

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_Includes_DefaultCondition()
        {
            var ci = new ClaimsIdentity(new Claim[] { 
                new Claim(ClaimTypes.NameIdentifier, "JohnDoe")
            });

            var a = ci.ToSaml2Assertion(new EntityId("http://idp.example.com"));

            // Default validity time is hearby defined to two minutes.
            a.Conditions.NotOnOrAfter.Value.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(2));
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_Includes_AudienceRestriction()
        {
            var ci = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "JohnDoe")
            });

            var audience = "http://sp.example.com/";

            var a = ci.ToSaml2Assertion(
                new EntityId("http://idp.example.com/"),
                new Uri(audience));

            a.Conditions.AudienceRestrictions.Should().HaveCount(1, "there should be one set of audience restrictions")
                .And.Subject.Single().Audiences.Should().HaveCount(1, "there should be one allowed audience")
                .And.Subject.Single().Should()
                .Be("http://sp.example.com/");
        }

        [TestMethod]
        public void ClaimsIdentitExtensions_ToSaml2NameIdentifier_Nullcheck()
        {
            Action a = () => ((ClaimsIdentity)null).ToSaml2NameIdentifier();

            a.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("identity");
        }
    }
}
