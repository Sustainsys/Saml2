using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IdentityModel.Tokens;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2SubjectExtensionsTests
    {
        [TestMethod]
        public void Saml2SubjectExtensions_ToXElement_CheckNull()
        {
            Saml2Subject subject = null;

            Action a = () => subject.ToXElement();

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("subject");
        }

        [TestMethod]
        public void Saml2SubjectExtensions_ToXElement()
        {
            var subjectName = "JohnDoe";
            var saml2Subject = new Saml2Subject(new Saml2NameIdentifier(subjectName));

            var subject = saml2Subject.ToXElement();

            subject.Element(Saml2Namespaces.Saml2 + "NameID").Value.Should().Be(subjectName);

            // Although SubjectConfirmation is optional in the SAML core spec, it is
            // mandatory in the Web Browser SSO Profile and must have a value of bearer.
            subject.Element(Saml2Namespaces.Saml2 + "SubjectConfirmation")
                .Attribute("Method").Value.Should().Be("urn:oasis:names:tc:SAML:2.0:cm:bearer");
        }
    }
}
