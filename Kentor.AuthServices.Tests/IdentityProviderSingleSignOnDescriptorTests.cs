using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.IdentityModel.Metadata;
using System.Xml.Linq;
using System.Linq;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class IdentityProviderSingleSignOnDescriptorTests
    {
        [TestMethod]
        public void IdentityProviderSingleSignOnDescriptor_NullcheckIdpSsoDescriptor()
        {
            Action a = () => ((IdentityProviderSingleSignOnDescriptor)null).Load(new XElement("Foo"));

            a.ShouldThrow<ArgumentNullException>("idpSsoDescriptor");
        }

        [TestMethod]
        public void IdentityProviderSingleSignOnDescriptor_NullcheckXmlData()
        {
            Action a = () => new IdentityProviderSingleSignOnDescriptor().Load(null);

            a.ShouldThrow<ArgumentNullException>("xmlData");
        }

        [TestMethod]
        public void IdentityProviderSingleSingOnDescriptor_CheckProtocolSupportEnumerationPresent()
        {
            var xml = new XElement(Saml2Namespaces.Saml2Metadata + "SingleSignOnService");

            Action a = () => new IdentityProviderSingleSignOnDescriptor().Load(xml);

            a.ShouldThrow<InvalidMetadataException>().And.Message.Should().Be("Missing protocolSupportEnumeration attribute in IDPSSODescriptor.");
        }

        [TestMethod]
        public void IdentityProviderSingleSignOnDescriptor_CheckProtocolSupportEnumerationIncorrect()
        {
            var xml = new XElement(Saml2Namespaces.Saml2Metadata + "SingleSignOnService",
                new XAttribute("protocolSupportEnumeration", "SomeInvalidData"));

            Action a = () => new IdentityProviderSingleSignOnDescriptor().Load(xml);

            a.ShouldThrow<InvalidMetadataException>().And.Message.Should().Be("Invalid protocolSupportEnumeration \"SomeInvalidData\".");
        }

        XElement CreateSingleSignOnServiceXml()
        {
            return new XElement(Saml2Namespaces.Saml2Metadata + "IDPSSODescriptor",
                new XAttribute("protocolSupportEnumeration", "urn:oasis:names:tc:SAML:2.0:protocol"),
                new XElement(Saml2Namespaces.Saml2Metadata + "SingleSignOnService",
                new XAttribute("Binding", Saml2Binding.HttpRedirectUri),
                new XAttribute("Location", "https://idp.example.com/ssoservice")));
        }

        [TestMethod]
        public void IdentityProviderSingleSignOnDescriptor_Load_SingleSignOnServiceContainsBinding()
        {
            var xml = CreateSingleSignOnServiceXml();

            var subject = new IdentityProviderSingleSignOnDescriptor();
            subject.Load(xml);

            subject.SingleSignOnServices.Single().Binding.Should().Be(Saml2Binding.HttpRedirectUri);
        }
    }
}
