using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System.IdentityModel.Metadata;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class ProtocolEndpointExtensionsTests
    {
        XElement CreateTestXml()
        {
            return new XElement(Saml2Namespaces.Saml2Metadata + "SingleSignOnService",
                new XAttribute("Binding", Saml2Binding.HttpRedirectUri),
                new XAttribute("Location", "https://idp.example.com/sso"));
        }

        [TestMethod]
        public void ProtocolEndpointExtensions_Load_MissingBinding()
        {
            var xml = CreateTestXml();
            xml.Attribute("Binding").Remove();

            var subject = new ProtocolEndpoint();

            Action a = () => subject.Load(xml);

            a.ShouldThrow<InvalidMetadataException>().And.Message.Should().Be("Missing Binding attribute in endpoint.");
        }

        [TestMethod]
        public void ProtocolEndpointExtensions_Load_Attributes()
        {
            var xml = CreateTestXml();

            var subject = new ProtocolEndpoint();
            subject.Load(xml);

            subject.Binding.Should().Be(Saml2Binding.HttpRedirectUri);
            subject.Location.Should().Be(new Uri("https://idp.example.com/sso"));
        }

        [TestMethod]
        public void ProtocolEndpointExtensions_Load_MissingLocation()
        {
            var xml = CreateTestXml();
            xml.Attribute("Location").Remove();

            var subject = new ProtocolEndpoint();

            Action a = () => subject.Load(xml);

            a.ShouldThrow<InvalidMetadataException>().And.Message.Should().Be("Missing Location attribute in endpoint.");
        }
    }
}
