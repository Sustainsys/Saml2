using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Collections.Generic;
using System.Security.Claims;
using Sustainsys.Saml2.Internal;
using System.Xml.Linq;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace Sustainsys.Saml2.Tests
{
    [TestClass]
    public class Saml2AssertionExtensionsTests
    {
        [TestMethod]
        public void Saml2AssertionExtensions_ToXElement_NullCheck()
        {
            Saml2Assertion assertion = null;

            Action a = () => assertion.ToXElement();

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("assertion");
        }

        [TestMethod]
        public void Saml2AssertionExtensions_ToXElement_Xml_BasicAttributes()
        {
            // Grab the current time before and after creating the assertion to
            // handle the unlikely event that the second part of the date time
            // is incremented during the test run. We don't want any heisenbugs.
            var before = DateTime.UtcNow.ToSaml2DateTimeString();

            var issuer = "http://idp.example.com";
            var assertion = new Saml2Assertion(new Saml2NameIdentifier(issuer));

            var after = DateTime.UtcNow.ToSaml2DateTimeString();

            var subject = assertion.ToXElement();

            subject.ToString().Should().StartWith(
                @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion");
            subject.Attribute("Version").Value.Should().Be("2.0");
            subject.Attribute("ID").Value.Should().NotBeNullOrWhiteSpace();
            subject.Attribute("IssueInstant").Value.Should().Match(
                i => i == before || i == after);
        }

        [TestMethod]
        public void Saml2AssertionExtensions_ToXElement_Issuer()
        {
            var issuer = "http://idp.example.com";
            var assertion = new Saml2Assertion(new Saml2NameIdentifier(issuer));

            var subject = assertion.ToXElement();

            subject.Element(Saml2Namespaces.Saml2 + "Issuer").Value.Should().Be(issuer);
        }

        [TestMethod]
        public void Saml2AssertionExtensions_ToXElement_Subject()
        {
            var subjectName = "JohnDoe";
            var assertion = new Saml2Assertion(
                new Saml2NameIdentifier("http://idp.example.com"))
                {
                    Subject = new Saml2Subject(new Saml2NameIdentifier(subjectName))
                };

            var subject = assertion.ToXElement();

            subject.Element(Saml2Namespaces.Saml2 + "Subject").
                Element(Saml2Namespaces.Saml2 + "NameID").Value.Should().Be(subjectName);
        }

        [TestMethod]
        public void Saml2AssertionExtensions_ToXElement_SubjectConfirmationData()
        {
            var subjectName = "JohnDoe";
            var destination = new Uri("http://sp.example.com");
            var inResponseTo = new Saml2Id("abc123");
            var notOnOrAfter = DateTime.UtcNow.AddMinutes(2);

            var assertion = new Saml2Assertion(
                new Saml2NameIdentifier("http://idp.example.com"))
            {
                Subject = new Saml2Subject(new Saml2NameIdentifier(subjectName))
                {
                    SubjectConfirmations =
                    {
                        new Saml2SubjectConfirmation(
                        new Uri("urn:oasis:names:tc:SAML:2.0:cm:bearer"),
                        new Saml2SubjectConfirmationData
                        {
                            NotOnOrAfter = notOnOrAfter,
                            InResponseTo = inResponseTo,
                            Recipient = destination
                        })
                    }
                }
            };

            var subject = assertion.ToXElement();

            var confirmationData = subject.Element(Saml2Namespaces.Saml2 + "Subject").
                Element(Saml2Namespaces.Saml2 + "SubjectConfirmation").
                Element(Saml2Namespaces.Saml2 + "SubjectConfirmationData");

            confirmationData.
                Attribute("Recipient").Value.Should().Be(destination.OriginalString);

            confirmationData.
                Attribute("NotOnOrAfter").Value.Should().Be(notOnOrAfter.ToSaml2DateTimeString());

            confirmationData.
                Attribute("InResponseTo").Value.Should().Be(inResponseTo.Value);
        }

        [TestMethod]
        public void Saml2AssertionExtensions_ToXElement_Conditions()
        {
            var assertion = new Saml2Assertion(
                new Saml2NameIdentifier("http://idp.example.com"))
            {
                Conditions = new Saml2Conditions()
                {
                    NotOnOrAfter = new DateTime(2099, 07, 25, 19, 52, 42, DateTimeKind.Utc)
                }
            };

            var subject = assertion.ToXElement();

            subject.Element(Saml2Namespaces.Saml2 + "Conditions")
                .Attribute("NotOnOrAfter").Value.Should().Be("2099-07-25T19:52:42Z");
        }

        [TestMethod]
        public void Saml2AssertionExtensions_ToXElement_Statements()
        {
            var attributeValue = "Test";
            var assertion = new Saml2Assertion(
                new Saml2NameIdentifier("http://idp.example.com"));

            assertion.Statements.Add(
                new Saml2AttributeStatement(new Saml2Attribute(ClaimTypes.Role, attributeValue)));

            assertion.Statements.Add(
                new Saml2AuthenticationStatement(
                    new Saml2AuthenticationContext(
                        new Uri("urn:oasis:names:tc:SAML:2.0:ac:classes:unspecified")))
                {
                    SessionIndex = "SessionIndex"
                });

            // Grab time before and after to use when comparing times. Even if
            // the time changes in the middle of the test, it should match one
            // of these times (unless you're debugging and spending more than
            // one second stepping through the code).
            var timeBefore = DateTime.UtcNow;
            var result = assertion.ToXElement();
            var timeAfter = DateTime.UtcNow;

            var actualAttributeXml = result.Element(Saml2Namespaces.Saml2 + "AttributeStatement");
            var expectedAttributeXml = new XElement(Saml2Namespaces.Saml2 + "AttributeStatement",
                new XElement(Saml2Namespaces.Saml2 + "Attribute",
                new XAttribute("Name", ClaimTypes.Role),
                new XElement(Saml2Namespaces.Saml2 + "AttributeValue",
                attributeValue)));
            actualAttributeXml.Should().BeEquivalentTo(expectedAttributeXml);

            var actualAuthnXml = result.Element(Saml2Namespaces.Saml2 + "AuthnStatement");

            // Compare time first and then drop it to avoid race issues
            var authnInstant = actualAuthnXml.Attribute("AuthnInstant");
            authnInstant.Value.Should().Match(
                d => d == timeBefore.ToSaml2DateTimeString() || d == timeAfter.ToSaml2DateTimeString());
            authnInstant.Remove();

            var expectedAuthnXml = new XElement(Saml2Namespaces.Saml2 + "AuthnStatement",
                new XAttribute("SessionIndex", "SessionIndex"),
                new XElement(Saml2Namespaces.Saml2 + "AuthnContext",
                    new XElement(Saml2Namespaces.Saml2 + "AuthnContextClassRef",
                        "urn:oasis:names:tc:SAML:2.0:ac:classes:unspecified")));

            actualAuthnXml.Should().BeEquivalentTo(expectedAuthnXml);
        }
    }
}
