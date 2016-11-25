using FluentAssertions;
using Kentor.AuthServices.Internal;
using Kentor.AuthServices.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;

namespace Kentor.AuthServices.Tests.Internal
{
    [TestClass]
    public class CryptographyExtensionsTests
    {
        [TestMethod]
        public void XmlHelpers_Encrypt_OaepTrue()
        {
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml("<xml />");
            var elementToEncrypt = xmlDoc.DocumentElement;
            elementToEncrypt.Encrypt(useOaep: true, certificate: SignedXmlHelper.TestCert);

            var nodes = new XmlElement[] { elementToEncrypt };
            nodes.Decrypt(SignedXmlHelper.TestCert.PrivateKey);

            nodes[0].OuterXml.Should().Be("<xml />");
        }

        [TestMethod]
        public void XmlHelpers_Encrypt_OaepFalse()
        {
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml("<xml />");
            var elementToEncrypt = xmlDoc.DocumentElement;
            elementToEncrypt.Encrypt(useOaep: false, certificate: SignedXmlHelper.TestCert);

            var nodes = new XmlElement[] { elementToEncrypt };
            nodes.Decrypt(SignedXmlHelper.TestCert.PrivateKey);

            nodes[0].OuterXml.Should().Be("<xml />");
        }

        [TestMethod]
        public void XmlHelpers_Encrypt_NullCert()
        {
            new XmlDocument().DocumentElement.Invoking(
                e => e.Encrypt(false, null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("certificate");
        }
    }
}
