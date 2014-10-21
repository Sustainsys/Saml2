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

namespace Kentor.AuthServices.Tests
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
        public void MetadataCommand_Run()
        {
            var subject = new MetadataCommand().Run(request, StubFactory.CreateOptions());

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
                new XElement(Saml2Namespaces.Saml2Metadata + "SPSSODescriptor",
                    new XAttribute("protocolSupportEnumeration", "urn:oasis:names:tc:SAML:2.0:protocol"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "AssertionConsumerService",
                        new XAttribute("Binding", Saml2Binding.HttpPostUri),
                        new XAttribute("Location", "http://localhost/AuthServices/Acs"),
                        new XAttribute("index", 0),
                        new XAttribute("isDefault", true))),
                new XElement(Saml2Namespaces.Saml2Metadata + "Organization",
                    new XElement(Saml2Namespaces.Saml2Metadata + "OrganizationName",
                        new XAttribute(XNamespace.Xml + "lang", ""), "Kentor.AuthServices"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "OrganizationDisplayName",
                        new XAttribute(XNamespace.Xml + "lang", ""), "Kentor AuthServices"),
                    new XElement(Saml2Namespaces.Saml2Metadata + "OrganizationURL",
                        new XAttribute(XNamespace.Xml + "lang" , ""), "http://github.com/KentorIT/authservices"))));

            payloadXml.ShouldBeEquivalentTo(expectedXml, opt => opt.IgnoringCyclicReferences());
            subject.ContentType.Should().Be("application/samlmetadata+xml");
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
