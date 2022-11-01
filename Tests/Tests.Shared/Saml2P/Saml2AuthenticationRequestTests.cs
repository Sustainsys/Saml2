﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Xml.Linq;
using System.Xml;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml2P;
using System.Linq;
using Microsoft.IdentityModel.Tokens.Saml2;
namespace Sustainsys.Saml2.Tests.Saml2P
{
    [TestClass]
    public class Saml2AuthenticationRequestTests
    {
        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_RootNode()
        {
            var subject = new Saml2AuthenticationRequest().ToXElement();

            subject.Should().NotBeNull().And.Subject.Name.Should().Be(
                Saml2Namespaces.Saml2P + "AuthnRequest");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_AddsRequestBaseFields()
        {
            // Just checking for the id field and assuming that means that the
            // base fields are added. The details of the fields are tested
            // by Saml2RequestBaseTests.

            var subject = new Saml2AuthenticationRequest().ToXElement();

            subject.Should().NotBeNull().And.Subject.Attribute("ID").Should().NotBeNull();
            subject.Attribute("AttributeConsumingServiceIndex").Should().BeNull();
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_AddsAttributeConsumingServiceIndex()
        {
            var subject = new Saml2AuthenticationRequest()
            {
                AttributeConsumingServiceIndex = 17
            }.ToXElement();

            subject.Attribute("AttributeConsumingServiceIndex").Value.Should().Be("17");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_AssertionConsumerServiceUrl()
        {
            string url = "http://some.example.com/Saml2AuthenticationModule/acs";
            var subject = new Saml2AuthenticationRequest()
            {
                AssertionConsumerServiceUrl = new Uri(url)
            }.ToXElement();

            subject.Should().NotBeNull().And.Subject.Attribute("AssertionConsumerServiceURL")
                .Should().NotBeNull().And.Subject.Value.Should().Be(url);
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ForceAuthentication_OmittedIfFalse()
        {
            var subject = new Saml2AuthenticationRequest()
            {
                ForceAuthentication = false
            }.ToXElement();

            subject.Should().NotBeNull().And.Subject.Attribute("ForceAuthn").Should().BeNull();
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ForceAuthentication()
        {
            var subject = new Saml2AuthenticationRequest()
            {
                ForceAuthentication = true
            }.ToXElement();

            subject.Should().NotBeNull().And.Subject.Attribute("ForceAuthn")
                .Should().NotBeNull().And.Subject.Value.Should().Be("true");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_IsPassive()
        {
            var subject = new Saml2AuthenticationRequest()
            {
                IsPassive = true
            }.ToXElement();

            subject.Should().NotBeNull().And.Subject.Attribute("IsPassive")
                .Should().NotBeNull().And.Subject.Value.Should().Be("true");
        }
        
        [TestMethod]
        public void Saml2AuthenticationRequest_Subject()
        {
            var subject = new Saml2AuthenticationRequest()
            {
                Subject = new Saml2Subject(new Saml2NameIdentifier("username"))
            }.ToXElement();
            
            subject.Should().NotBeNull()
                .And.Subject.Element(XNamespace.Get(Saml2Namespaces.Saml2Name) + "Subject").Should().NotBeNull()
                .And.Subject.Elements().Should().HaveCount(2)
                .And.Subject.First().Name.LocalName.Should().Be("NameID");
            subject.Element(XNamespace.Get(Saml2Namespaces.Saml2Name) + "Subject")?.Elements().Last().Name.LocalName.Should().Be("SubjectConfirmation");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_Extensions()
        {
            var request = new Saml2AuthenticationRequest();
            request.ExtensionContents.Add(new XElement(XNamespace.Get("test") + "aditional"));
            var subject = request.ToXElement();

            subject.Should().NotBeNull().And.Subject
                .Element(Saml2Namespaces.Saml2P + "Extensions").Should().NotBeNull().And.Subject
                .Elements().Should().HaveCount(1).And.Subject
                .First().Name.LocalName.Should().Be("aditional");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_Read()
        {
            var xmlData = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<samlp:AuthnRequest
  xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
  xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion""
  ID=""Saml2AuthenticationRequest_AssertionConsumerServiceUrl""
  Version=""2.0""
  Destination=""http://destination.example.com""
  AssertionConsumerServiceURL=""https://sp.example.com/SAML2/Acs""
  IssueInstant=""2004-12-05T09:21:59Z"">
  <saml:Issuer>https://sp.example.com/SAML2</saml:Issuer>
/>
</samlp:AuthnRequest>
";

            var relayState = "My relay state";
            var subject = Saml2AuthenticationRequest.Read(xmlData, relayState);

            subject.Id.Value.Should().Be("Saml2AuthenticationRequest_AssertionConsumerServiceUrl");
            subject.AssertionConsumerServiceUrl.Should().Be(new Uri("https://sp.example.com/SAML2/Acs"));
            subject.RelayState.Should().Be(relayState);
            subject.ForceAuthentication.Should().BeFalse();
            subject.IsPassive.Should().BeFalse();
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_Read_FlagsSet()
        {
            var xmlData = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<samlp:AuthnRequest
  xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
  xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion""
  ID=""Saml2AuthenticationRequest_AssertionConsumerServiceUrl""
  Version=""2.0""
  Destination=""http://destination.example.com""
  AssertionConsumerServiceURL=""https://sp.example.com/SAML2/Acs""
  IssueInstant=""2004-12-05T09:21:59Z""
  ForceAuthn=""true""
  IsPassive=""1"">
  <saml:Issuer>https://sp.example.com/SAML2</saml:Issuer>
/>
</samlp:AuthnRequest>
";

            var relayState = "My relay state";
            var subject = Saml2AuthenticationRequest.Read(xmlData, relayState);

            subject.Id.Value.Should().Be("Saml2AuthenticationRequest_AssertionConsumerServiceUrl");
            subject.AssertionConsumerServiceUrl.Should().Be(new Uri("https://sp.example.com/SAML2/Acs"));
            subject.RelayState.Should().Be(relayState);
            subject.ForceAuthentication.Should().BeTrue();
            subject.IsPassive.Should().BeTrue();
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_Read_NoACS()
        {
            var xmlData = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<samlp:AuthnRequest
  xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
  xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion""
  ID=""Saml2AuthenticationRequest_Read_NoACS""
  Version=""2.0""
  Destination=""http://destination.example.com""
  IssueInstant=""2004-12-05T09:21:59Z""
  ForceAuthn=""false"">
  <saml:Issuer>https://sp.example.com/SAML2</saml:Issuer>
/>
</samlp:AuthnRequest>
";

            var subject = Saml2AuthenticationRequest.Read(xmlData, null);

            subject.Id.Value.Should().Be("Saml2AuthenticationRequest_Read_NoACS");
            subject.AssertionConsumerServiceUrl.Should().Be(null);
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_Read_ShouldThrowOnInvalidVersion()
        {
            var xmlData = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<samlp:AuthnRequest
  xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
  xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion""
  ID=""Saml2AuthenticationRequest_Read_Should().ThrowOnInvalidVersion""
  Version=""123456789.0""
  Destination=""http://destination.example.com""
  AssertionConsumerServiceURL=""https://sp.example.com/SAML2/Acs""
  IssueInstant=""2004-12-05T09:21:59Z""
  InResponseTo=""111222333"">
  <saml:Issuer>https://sp.example.com/SAML2</saml:Issuer>
/>
</samlp:AuthnRequest>
";

            Action a = () => Saml2AuthenticationRequest.Read(xmlData, null);

            a.Should().Throw<XmlException>().WithMessage("Wrong or unsupported SAML2 version");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_Read_ShouldThrowOnInvalidMessageName()
        {
            var xmlData = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<samlp:NotAuthnRequest
  xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
  xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion""
  ID=""Saml2AuthenticationRequest_Read_Should().ThrowOnInvalidMessageName""
  Version=""2.0""
  Destination=""http://destination.example.com""
  AssertionConsumerServiceURL=""https://sp.example.com/SAML2/Acs""
  IssueInstant=""2004-12-05T09:21:59Z""
  InResponseTo=""111222333"">
  <saml:Issuer>https://sp.example.com/SAML2</saml:Issuer>
/>
</samlp:NotAuthnRequest>
";

            Action a = () => Saml2AuthenticationRequest.Read(xmlData, null);

            a.Should().Throw<XmlException>().WithMessage("Expected a SAML2 authentication request document");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_Read_NameIdPolicy()
        {
            var xmlData = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<saml2p:AuthnRequest xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                     xmlns:saml2 =""urn:oasis:names:tc:SAML:2.0:assertion""
                     ID=""ide3c2f1c88255463ab4eb1b158fa6f616""
                     Version=""2.0""
                     IssueInstant=""2016-01-25T13:01:09Z""
                     Destination=""http://destination.example.com""
                     AssertionConsumerServiceURL=""https://sp.example.com/SAML2/Acs""
                     >
    <saml2:Issuer>https://sp.example.com/SAML2</saml2:Issuer>
    <saml2p:NameIDPolicy AllowCreate = ""false"" Format = ""urn:oasis:names:tc:SAML:2.0:nameid-format:persistent"" />
   </saml2p:AuthnRequest>";

            var subject = Saml2AuthenticationRequest.Read(xmlData, null);
            subject.NameIdPolicy.AllowCreate.Should().Be(false);
            subject.NameIdPolicy.Format.Should().Be(NameIdFormat.Persistent);
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_Read_NoFormat()
        {
            var xmlData = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<saml2p:AuthnRequest xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                     xmlns:saml2 =""urn:oasis:names:tc:SAML:2.0:assertion""
                     ID=""ide3c2f1c88255463ab4eb1b158fa6f616""
                     Version=""2.0""
                     IssueInstant=""2016-01-25T13:01:09Z""
                     Destination=""http://destination.example.com""
                     AssertionConsumerServiceURL=""https://sp.example.com/SAML2/Acs""
                     >
    <saml2:Issuer>https://sp.example.com/SAML2</saml2:Issuer>
    <saml2p:NameIDPolicy AllowCreate = ""false""/>
   </saml2p:AuthnRequest>";

            var subject = Saml2AuthenticationRequest.Read(xmlData, null);
            subject.NameIdPolicy.AllowCreate.Should().Be(false);
            subject.NameIdPolicy.Format.Should().Be(NameIdFormat.NotConfigured);
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_Read_Extensions()
        {
            var xmlData = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<samlp:AuthnRequest
                     xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
                     xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion""
                     ID=""Saml2AuthenticationRequest_AssertionConsumerServiceUrl""
                     Version=""2.0""
                     Destination=""http://destination.example.com""
                     AssertionConsumerServiceURL=""https://sp.example.com/SAML2/Acs""
                     IssueInstant=""2004-12-05T09:21:59Z""
                     ForceAuthn=""true"">
    <saml:Issuer>https://sp.example.com/SAML2</saml:Issuer>
    <samlp:Extensions>
      <additional xmlns=""testurn:test"" />
    </samlp:Extensions>
</samlp:AuthnRequest>
";

            var subject = Saml2AuthenticationRequest.Read(xmlData, null);
            subject.ExtensionContents.Should().HaveCount(1);
            subject.ExtensionContents[0].ToString().Should().BeEquivalentTo(@"<additional xmlns=""testurn:test"" />");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_AddsElementSaml2NameIdPolicy_ForAllowCreate()
        {
            var subject = new Saml2AuthenticationRequest()
            {
                AssertionConsumerServiceUrl = new Uri("http://destination.example.com"),
                NameIdPolicy = new Saml2NameIdPolicy(false, NameIdFormat.NotConfigured)
            }.ToXElement();

            var expected = new XElement(Saml2Namespaces.Saml2P + "root",
                new XAttribute(XNamespace.Xmlns + "saml2p", Saml2Namespaces.Saml2P),
                new XElement(Saml2Namespaces.Saml2P + "NameIDPolicy",
                    new XAttribute("AllowCreate", false)))
                    .Elements().Single();

            subject.Attribute("AttributeConsumingServiceIndex").Should().BeNull();
            subject.Element(Saml2Namespaces.Saml2P + "NameIDPolicy")
                .Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_AddsElementSaml2NameIdPolicy_ForNameIdFormat()
        {
            var subject = new Saml2AuthenticationRequest()
            {
                AssertionConsumerServiceUrl = new Uri("http://destination.example.com"),
                NameIdPolicy = new Saml2NameIdPolicy(null, NameIdFormat.EmailAddress)
            }.ToXElement();

            var expected = new XElement(Saml2Namespaces.Saml2P + "root",
                new XAttribute(XNamespace.Xmlns + "saml2p", Saml2Namespaces.Saml2P),
                new XElement(Saml2Namespaces.Saml2P + "NameIDPolicy",
                    new XAttribute("Format", "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress")))
                    .Elements().Single();

            subject.Element(Saml2Namespaces.Saml2P + "NameIDPolicy")
                .Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_AddsScoping()
        {
            var requesterId = "urn://requesterid/";
            var location = "http://location";
            var name = "name";
            var providerId = "urn:providerId";

            var subject = new Saml2AuthenticationRequest()
            {
                AssertionConsumerServiceUrl = new Uri("http://destination.example.com"),
                Scoping = new Saml2Scoping()
                {
                    ProxyCount = 5
                }
                .With(new Saml2IdpEntry(new EntityId(providerId))
                {
                    Name = name,
                    Location = new Uri(location)
                })
                .WithRequesterId(new EntityId(requesterId))
            };

            var actual = subject.ToXElement().Element(Saml2Namespaces.Saml2P + "Scoping");

            var expected = new XElement(Saml2Namespaces.Saml2P + "Scoping",
                    new XAttribute("ProxyCount", "5"),
                    new XElement(Saml2Namespaces.Saml2P + "IDPList",
                        new XElement(Saml2Namespaces.Saml2P + "IDPEntry",
                            new XAttribute("ProviderID", providerId),
                            new XAttribute("Name", name),
                            new XAttribute("Loc", location))),
                    new XElement(Saml2Namespaces.Saml2P + "RequesterID", requesterId.ToString()));

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_Scoping_ZeroProxyCount_AttributeAdded()
        {
            var subject = new Saml2AuthenticationRequest()
            {
                AssertionConsumerServiceUrl = new Uri("http://destination.example.com"),
                Scoping = new Saml2Scoping()
                {
                    ProxyCount = 0
                }
            };

            var actual = subject.ToXElement().Element(Saml2Namespaces.Saml2P + "Scoping");

            var expected = new XElement(Saml2Namespaces.Saml2P + "root",
                new XAttribute(XNamespace.Xmlns + "saml2p", Saml2Namespaces.Saml2P),
                new XElement(Saml2Namespaces.Saml2P + "Scoping",
                 new XAttribute("ProxyCount", "0")))
                .Elements().Single();

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_Scoping_NullContents_EmptyScoping()
        {
            var subject = new Saml2AuthenticationRequest()
            {
                AssertionConsumerServiceUrl = new Uri("http://destination.example.com"),
                Scoping = new Saml2Scoping()
            }.ToXElement().Element(Saml2Namespaces.Saml2P + "Scoping");

            var expected = new XElement(Saml2Namespaces.Saml2P + "root",
                new XAttribute(XNamespace.Xmlns + "saml2p", Saml2Namespaces.Saml2P),
                new XElement(Saml2Namespaces.Saml2P + "Scoping"))
                .Elements().Single();

            subject.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_AddsRequestedAuthnContext_ComparisonTypeMaximum()
        {
            Saml2AuthenticationRequest_ToXElement_AddsRequestedAuthnContextUtil(AuthnContextComparisonType.Maximum, "maximum");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_AddsRequestedAuthnContext_ComparisonTypeExact()
        {
            Saml2AuthenticationRequest_ToXElement_AddsRequestedAuthnContextUtil(AuthnContextComparisonType.Exact, "exact");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_AddsRequestedAuthnContext_ComparisonTypeMinimum()
        {
            Saml2AuthenticationRequest_ToXElement_AddsRequestedAuthnContextUtil(AuthnContextComparisonType.Minimum, "minimum");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_AddsRequestedAuthnContext_ComparisonTypeBetter()
        {
            Saml2AuthenticationRequest_ToXElement_AddsRequestedAuthnContextUtil(AuthnContextComparisonType.Better, "better");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_AddsProtocolBinding_HttpPost()
        {
            Saml2AuthenticationRequest_ToXElement_AddsProtocolBinding(Saml2.WebSso.Saml2BindingType.HttpPost, "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_AddsProtocolBinding_Artifact()
        {
            Saml2AuthenticationRequest_ToXElement_AddsProtocolBinding(Saml2.WebSso.Saml2BindingType.Artifact, "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_OmitsRequestedAuthnContext_OnNullClassRef()
        {
            var subject = new Saml2AuthenticationRequest()
            {
                AssertionConsumerServiceUrl = new Uri("http://destination.example.com"),
                RequestedAuthnContext = new Saml2RequestedAuthnContext(null, AuthnContextComparisonType.Exact)
            }.ToXElement();

            subject.Element(Saml2Namespaces.Saml2P + "RequestedAuthnContext").Should().BeNull();
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_NameFormatTransientForbidsAllowCreate()
        {
            var subject = new Saml2AuthenticationRequest()
            {
                AssertionConsumerServiceUrl = new Uri("http://destination.example.com"),
                NameIdPolicy = new Saml2NameIdPolicy(true, NameIdFormat.Transient)
            };

            subject.Invoking(s => s.ToXElement())
                .Should().Throw<InvalidOperationException>()
                .And.Message.Should().Be("When NameIdPolicy/Format is set to Transient, it is not permitted to specify AllowCreate. Change Format or leave AllowCreate as null.");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_Read_ShouldReturnNullOnNullXml()
        {
            string xmlData = null;

            var subject = Saml2AuthenticationRequest.Read(xmlData, null);

            subject.Should().BeNull();
        }

        private void Saml2AuthenticationRequest_ToXElement_AddsRequestedAuthnContextUtil(AuthnContextComparisonType comparisonType, string expectedComparisonType)
        {
            var classRef = "http://www.Sustainsys.se";
            var subject = new Saml2AuthenticationRequest()
            {
                AssertionConsumerServiceUrl = new Uri("http://destination.example.com"),
                RequestedAuthnContext = new Saml2RequestedAuthnContext(new Uri(classRef), comparisonType)
            }.ToXElement();

            var expected = new XElement(Saml2Namespaces.Saml2P + "root",
                new XAttribute(XNamespace.Xmlns + "saml2p", Saml2Namespaces.Saml2P),
                new XAttribute(XNamespace.Xmlns + "saml2", Saml2Namespaces.Saml2),
                new XElement(Saml2Namespaces.Saml2P + "RequestedAuthnContext",
                    new XAttribute("Comparison", expectedComparisonType),
                    new XElement(Saml2Namespaces.Saml2 + "AuthnContextClassRef", classRef)))
                    .Elements().Single();

            var actual = subject.Element(Saml2Namespaces.Saml2P + "RequestedAuthnContext");

            actual.Should().BeEquivalentTo(expected);
        }

        private void Saml2AuthenticationRequest_ToXElement_AddsProtocolBinding(Saml2.WebSso.Saml2BindingType protocolBinding, string expectedProtocolBinding)
        {
            var subject = new Saml2AuthenticationRequest()
            {
                AssertionConsumerServiceUrl = new Uri("http://destination.example.com"),
                Binding = protocolBinding
            }.ToXElement();

            subject.Attribute("ProtocolBinding").Value.Should().Be(expectedProtocolBinding);
        }
    }
}
