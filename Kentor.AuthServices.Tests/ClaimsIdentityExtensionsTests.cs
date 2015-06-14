using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using FluentAssertions;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class ClaimsIdentityExtensionsTests
    {
        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_ThrowsOnNullIdentity()
        {
            ClaimsIdentity identity = null;

            Action a = () => identity.ToSaml2Assertion(new EntityId("foo"));

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("identity");
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_ThrowsOnNullIssuer()
        {
            var subject = new ClaimsIdentity();

            Action a = () => subject.ToSaml2Assertion(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("issuer");
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
        public void ClaimsIdentityExtensions_ToSaml2Assertion_Includes_Attributes()
        {
            var ci = new ClaimsIdentity(new Claim[] { 
                new Claim(ClaimTypes.NameIdentifier, "JohnDoe"),
                new Claim(ClaimTypes.Role, "Test")
            });

            var a = ci.ToSaml2Assertion(new EntityId("http://idp.example.com"));

            a.Statements.SingleOrDefault().Should().BeOfType<Saml2AttributeStatement>();
            (a.Statements.SingleOrDefault() as Saml2AttributeStatement).Attributes[0].Values[0].Should().Be("Test");
        }

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_MultipleValuesForSameKey_CombinesTo_OneAttribute()
        {
            var ci = new ClaimsIdentity(new Claim[] { 
                new Claim(ClaimTypes.NameIdentifier, "JohnDoe"),
                new Claim(ClaimTypes.Role, "Test1"),
                new Claim(ClaimTypes.Role, "Test2"),
            });

            var a = ci.ToSaml2Assertion(new EntityId("http://idp.example.com"));

            a.Statements.SingleOrDefault().Should().BeOfType<Saml2AttributeStatement>();
            (a.Statements.SingleOrDefault() as Saml2AttributeStatement).Attributes[0].Values[0].Should().Be("Test1");
            (a.Statements.SingleOrDefault() as Saml2AttributeStatement).Attributes[0].Values[1].Should().Be("Test2");
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
    }
}
