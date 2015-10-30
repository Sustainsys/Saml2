using System;
using System.Web;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using FluentAssertions;
using System.IdentityModel.Metadata;
using System.Xml;
using Kentor.AuthServices.Configuration;
using System.Globalization;
using Kentor.AuthServices.WebSso;

namespace Kentor.AuthServices.Tests.WebSso
{
    [TestClass]
    public class MetadataCommandTests
    {
        [TestMethod]
        public void MetadataCommand_Run_NullcheckOptions()
        {
            Action a = () => new MetadataCommand().Run(
                new HttpRequestData("GET", new Uri("http://localhost")), 
                null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }

        HttpRequestData request = new HttpRequestData("GET", new Uri("http://localhost"));

        [TestMethod]
        public void MetadataCommand_Run_CompleteMetadata()
        {
            var options = StubFactory.CreateOptions();
            ((SPOptions)options.SPOptions).DiscoveryServiceUrl = new Uri("http://ds.example.com");

            var subject = new MetadataCommand().Run(request, options);

            XDocument payloadXml = XDocument.Parse(subject.Content);

            // Ignore the ID attribute, it is just filled with a GUID that can't be easily tested.
            payloadXml.Root.Attribute("ID").Remove();

            var expectedXml = new XDocument(new XElement(Saml2Namespaces.Saml2Metadata + "EntityDescriptor",
                new XAttribute("entityID", "https://github.com/KentorIT/authservices"),
                new XAttribute("cacheDuration", "PT42S"),
                // Have to manually add the xmlns attribute here, as it will be present in the subject
                // data and the xml tree comparison will fail if it is not present in both. Just setting the 
                // namespace of the elements does not inject the xmlns attribute into the node tree. It is
                // only done when outputting a string.
                // See http://stackoverflow.com/questions/24156689/xnode-deepequals-unexpectedly-returns-false
                new XAttribute("xmlns", Saml2Namespaces.Saml2MetadataName),
                new XAttribute(XNamespace.Xmlns + "saml2", Saml2Namespaces.Saml2),
                new XElement(Saml2Namespaces.Saml2Metadata + "SPSSODescriptor",
                    new XAttribute("protocolSupportEnumeration", "urn:oasis:names:tc:SAML:2.0:protocol"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "Extensions",
                        new XElement(Saml2Namespaces.Saml2IdpDiscovery + "DiscoveryResponse",
                            new XAttribute("Binding", Saml2Binding.DiscoveryResponseUri),
                            new XAttribute("Location", "http://localhost/AuthServices/SignIn"),
                            new XAttribute("index", 0),
                            new XAttribute("isDefault", true),
                            new XAttribute("xmlns", Saml2Namespaces.Saml2IdpDiscoveryName))),
                    new XElement(Saml2Namespaces.Saml2Metadata + "AssertionConsumerService",
                        new XAttribute("Binding", Saml2Binding.HttpPostUri),
                        new XAttribute("Location", "http://localhost/AuthServices/Acs"),
                        new XAttribute("index", 0),
                        new XAttribute("isDefault", true)),
                    new XElement(Saml2Namespaces.Saml2Metadata + "AttributeConsumingService",
                        new XAttribute("index", 0),
                        new XAttribute("isDefault", true),
                        new XElement(Saml2Namespaces.Saml2Metadata + "ServiceName",
                            new XAttribute(XNamespace.Xml + "lang", "en"),
                            "attributeServiceName"),
                        new XElement(Saml2Namespaces.Saml2Metadata + "RequestedAttribute",
                            new XAttribute("Name", "urn:attributeName"),
                            new XAttribute("isRequired", "true"),
                            new XAttribute("NameFormat", "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),
                            new XAttribute("FriendlyName", "friendlyName"),
                            new XElement(Saml2Namespaces.Saml2 + "AttributeValue", "value1"),
                            new XElement(Saml2Namespaces.Saml2 + "AttributeValue", "value2")),
                        new XElement(Saml2Namespaces.Saml2Metadata + "RequestedAttribute",
                            new XAttribute("Name", "someName"),
                            new XAttribute("isRequired", "false")))),
                new XElement(Saml2Namespaces.Saml2Metadata + "Organization",
                    new XElement(Saml2Namespaces.Saml2Metadata + "OrganizationName",
                        new XAttribute(XNamespace.Xml + "lang", ""), "Kentor.AuthServices"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "OrganizationDisplayName",
                        new XAttribute(XNamespace.Xml + "lang", ""), "Kentor AuthServices"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "OrganizationURL",
                        new XAttribute(XNamespace.Xml + "lang", ""), "http://github.com/KentorIT/authservices")),
                new XElement(Saml2Namespaces.Saml2Metadata + "ContactPerson",
                    new XAttribute("contactType", "support"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "Company", "Kentor"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "GivenName", "Anders"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "SurName", "Abel"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "EmailAddress", "info@kentor.se"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "EmailAddress", "anders.abel@kentor.se"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "TelephoneNumber", "+46 8 587 650 00"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "TelephoneNumber", "+46 708 96 50 63")),
                new XElement(Saml2Namespaces.Saml2Metadata + "ContactPerson",
                    new XAttribute("contactType", "technical"))));

            payloadXml.ShouldBeEquivalentTo(expectedXml, opt => opt.IgnoringCyclicReferences());
            subject.ContentType.Should().Be("application/samlmetadata+xml");
        }

        [TestMethod]
        public void MetadataCommand_Run_MinimalMetadata()
        {
            var spOptions = new SPOptions()
            {
                EntityId = new EntityId("http://localhost/AuthServices"),
            };
            var options = new Options(spOptions);

            var subject = new MetadataCommand().Run(request, options);

            XDocument payloadXml = XDocument.Parse(subject.Content);

            // Ignore the ID attribute, it is just filled with a GUID that can't be easily tested.
            payloadXml.Root.Attribute("ID").Remove();

            var expectedXml = new XDocument(new XElement(Saml2Namespaces.Saml2Metadata + "EntityDescriptor",
                new XAttribute("entityID", "http://localhost/AuthServices"),
                new XAttribute("cacheDuration", "PT1H"),
                // Have to manually add the xmlns attribute here, as it will be present in the subject
                // data and the xml tree comparison will fail if it is not present in both. Just setting the 
                // namespace of the elements does not inject the xmlns attribute into the node tree. It is
                // only done when outputting a string.
                // See http://stackoverflow.com/questions/24156689/xnode-deepequals-unexpectedly-returns-false
                new XAttribute("xmlns", Saml2Namespaces.Saml2MetadataName),
                new XAttribute(XNamespace.Xmlns + "saml2", Saml2Namespaces.Saml2),
                new XElement(Saml2Namespaces.Saml2Metadata + "SPSSODescriptor",
                    new XAttribute("protocolSupportEnumeration", "urn:oasis:names:tc:SAML:2.0:protocol"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "AssertionConsumerService",
                        new XAttribute("Binding", Saml2Binding.HttpPostUri),
                        new XAttribute("Location", "http://localhost/AuthServices/Acs"),
                        new XAttribute("index", 0),
                        new XAttribute("isDefault", true)))));

            payloadXml.ShouldBeEquivalentTo(expectedXml, opt => opt.IgnoringCyclicReferences());
        }

        [TestMethod]
        public void MetadataCommand_Run_ThrowsOnMissingOrganizationDisplayName()
        {
            var options = StubFactory.CreateOptions();

            options.SPOptions.Organization.DisplayNames.Clear();

            Action a = () => new MetadataCommand().Run(request, options);

            a.ShouldThrow<MetadataSerializationException>().And.Message.Should().StartWith("ID3203");
        }
    }
}
