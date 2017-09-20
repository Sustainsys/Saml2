using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IdentityModel.Tokens;
using FluentAssertions;
using System.Linq;
using System.Xml.Linq;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2ConditionsExtensionsTests
    {
        [TestMethod]
        public void Saml2ConditionsExtensions_ToXElement_Nullcheck()
        {
            Action a = () => ((Saml2Conditions)null).ToXElement();

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("conditions");
        }

        [TestMethod]
        public void Saml2ConditionsExtensions_ToXElement_OnlyNotOnOrAfter()
        {
            var conditions = new Saml2Conditions()
            {
                NotOnOrAfter = new DateTime(2099, 07, 25, 19, 52, 42, DateTimeKind.Utc)
            };

            var actual = conditions.ToXElement();

            actual.Name.Should().Be(Saml2Namespaces.Saml2 + "Conditions");

            actual.Attribute("NotOnOrAfter").Value.Should().Be("2099-07-25T19:52:42Z");
        }

        [TestMethod]
        public void Saml2ConditionsExtensions_ToXElement_OnlyAudienceRestriction()
        {
            var conditions = new Saml2Conditions();
            conditions.AudienceRestrictions.Add(new Saml2AudienceRestriction(new[]
            {
                new Uri("http://foo1"),
                new Uri("http://foo2")
            }));

            conditions.AudienceRestrictions.Add(new Saml2AudienceRestriction(new Uri("http://bar")));

            var actual = conditions.ToXElement();

            var expected = new XElement(Saml2Namespaces.Saml2 + "Conditions",
                new XElement(Saml2Namespaces.Saml2 + "AudienceRestriction",
                    new XElement(Saml2Namespaces.Saml2 + "Audience", "http://foo1"),
                    new XElement(Saml2Namespaces.Saml2 + "Audience", "http://foo2")),
                new XElement(Saml2Namespaces.Saml2 + "AudienceRestriction",
                    new XElement(Saml2Namespaces.Saml2 + "Audience", "http://bar")));

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
