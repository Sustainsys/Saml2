using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IdentityModel.Tokens;
using FluentAssertions;

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
        public void Saml2ConditionsExtensions_ToXElement()
        {
            var conditions = new Saml2Conditions()
            {
                NotOnOrAfter = new DateTime(2099, 07, 25, 19, 52, 42, DateTimeKind.Utc)
            };

            var subject = conditions.ToXElement();

            subject.Name.Should().Be(Saml2Namespaces.Saml2 + "Conditions");

            subject.Attribute("NotOnOrAfter").Value.Should().Be("2099-07-25T19:52:42Z");
        }
    }
}
