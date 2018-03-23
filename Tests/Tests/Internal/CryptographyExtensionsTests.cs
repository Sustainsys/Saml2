using FluentAssertions;
using Sustainsys.Saml2.Internal;
using Sustainsys.Saml2.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Internal
{
    [TestClass]
    public class CryptographyExtensionsTests
    {
        [TestMethod]
        public void XmlHelpers_Encrypt_OaepTrue()
        {
            var xmlDoc = XmlHelpers.XmlDocumentFromString("<xml />");
            var elementToEncrypt = xmlDoc.DocumentElement;
            elementToEncrypt.Encrypt(useOaep: true, certificate: SignedXmlHelper.TestCert);

            var nodes = new XmlElement[] { elementToEncrypt };
            nodes.Decrypt(SignedXmlHelper.TestCert.PrivateKey);

            nodes[0].OuterXml.Should().Be("<xml />");
        }

        [TestMethod]
        public void XmlHelpers_Encrypt_OaepFalse()
        {
            var xmlDoc = XmlHelpers.XmlDocumentFromString("<xml />");
            var elementToEncrypt = xmlDoc.DocumentElement;
            elementToEncrypt.Encrypt(useOaep: false, certificate: SignedXmlHelper.TestCert);

            var nodes = new XmlElement[] { elementToEncrypt };
            nodes.Decrypt(SignedXmlHelper.TestCert.PrivateKey);

            nodes[0].OuterXml.Should().Be("<xml />");
        }

        [TestMethod]
        public void XmlHelpers_Encrypt_NullCert()
        {
            XmlHelpers.CreateSafeXmlDocument().DocumentElement.Invoking(
                e => e.Encrypt(false, null))
                .Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("certificate");
        }
    }
}
