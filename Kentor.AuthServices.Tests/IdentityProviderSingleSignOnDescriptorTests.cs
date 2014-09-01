using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.IdentityModel.Metadata;
using System.Xml.Linq;

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

            a.ShouldThrow<InvalidMetadataException>().And.Message.Should().Be("Missing protocolSupportEnumeration attribute in SingleSignOnService.");
        }

        [TestMethod]
        public void IdentityProviderSingleSignOnDescriptor_CheckProtocolSupportEnumerationIncorrect()
        {
            var xml = new XElement(Saml2Namespaces.Saml2Metadata + "SingleSignOnService",
                new XAttribute("protocolSupportEnumeration", "SomeInvalidData"));

            Action a = () => new IdentityProviderSingleSignOnDescriptor().Load(xml);

            a.ShouldThrow<InvalidMetadataException>().And.Message.Should().Be("Invalid protocolSupportEnumeration \"SomeInvalidData\".");
        }

        [TestMethod]
        public void IdentityProviderSingleSignOnDescriptor_CheckProtocolSupportEnumerationCorrect()
        {
            var xml = new XElement(Saml2Namespaces.Saml2Metadata + "SingleSignOnService",
                new XAttribute("protocolSupportEnumeration", "urn:oasis:names:tc:SAML:2.0:protocol"));

            Action a = () => new IdentityProviderSingleSignOnDescriptor().Load(xml);

            a.ShouldNotThrow();
        }
    }
}
