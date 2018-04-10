using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.IdentityModel.Tokens.Saml2;
using FluentAssertions;

namespace Sustainsys.Saml2.Tests
{
    [TestClass]
    public class Saml2SubjectExtensionsTests
    {
        [TestMethod]
        public void Saml2SubjectExtensions_ToXElement_CheckNull()
        {
            Saml2Subject subject = null;

            Action a = () => subject.ToXElement();

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("subject");
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

        [TestMethod]
        public void Saml2SubjectExtensions_ToXElement_SubjectConfirmation_CheckNull()
        {
            Saml2SubjectConfirmation saml2SubjectConfirmation = null;

            Action a = () => saml2SubjectConfirmation.ToXElement();

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("subjectConfirmation");
        }

        [TestMethod]
        public void Saml2SubjectExtensions_ToXElement_SubjectConfirmation()
        {
            var saml2SubjectConfirmation = new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:bearer"));

            var confirmation = saml2SubjectConfirmation.ToXElement();

            confirmation.Attribute("Method").Value.Should().Be("urn:oasis:names:tc:SAML:2.0:cm:bearer");
        }

        [TestMethod]
        public void Saml2SubjectExtensions_ToXElement_SubjectConfirmationData_CheckNull()
        {
            Saml2SubjectConfirmationData saml2SubjectConfirmationData = null;

            Action a = () => saml2SubjectConfirmationData.ToXElement();

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("subjectConfirmationData");
        }

        [TestMethod]
        public void Saml2SubjectExtensions_ToXElement_SubjectConfirmationData()
        {
            var destination = new Uri("http://sp.example.com");
            var inResponseTo = new Saml2Id("abc123");
            var notOnOrAfter = DateTime.UtcNow.AddMinutes(2);
            var notBefore = DateTime.UtcNow;

            var saml2SubjectConfirmationData = new Saml2SubjectConfirmationData
            {
                NotOnOrAfter = notOnOrAfter,
                InResponseTo = inResponseTo,
                Recipient = destination,
                NotBefore = notBefore
            };

            var confirmation = saml2SubjectConfirmationData.ToXElement();

            confirmation.Attribute("NotOnOrAfter").Value.Should().Be(notOnOrAfter.ToSaml2DateTimeString());

            confirmation.Attribute("NotBefore").Value.Should().Be(notBefore.ToSaml2DateTimeString());

            confirmation.Attribute("Recipient").Value.Should().Be(destination.OriginalString);

            confirmation.Attribute("InResponseTo").Value.Should().Be(inResponseTo.Value);
        }
    }
}
