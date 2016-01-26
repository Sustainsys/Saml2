using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class XmlDocumentHelpersTests
    {
        public static readonly X509Certificate2 TestCert = new X509Certificate2("Kentor.AuthServices.Tests.pfx");

        [TestMethod]
        public void XmlDocumentHelpers_Sign_Nullcheck_xmlDocument()
        {
            XmlDocument xd = null;
            Action a = () => xd.Sign(TestCert);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("xmlDocument");
        }

        [TestMethod]
        public void XmlDocumentHelpers_Sign_Nullcheck_Cert()
        {
            XmlDocument xd = new XmlDocument();
            Action a = () => xd.Sign(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("cert");
        }

        [TestMethod]
        public void XmlDocumentHelpers_Sign()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<root ID=\"rootElementId\"><content>Some Content</content></root>");

            xmlDoc.Sign(TestCert);

            var signature = xmlDoc.DocumentElement["Signature", SignedXml.XmlDsigNamespaceUrl];

            signature["SignedInfo", SignedXml.XmlDsigNamespaceUrl]
                ["Reference", SignedXml.XmlDsigNamespaceUrl].Attributes["URI"].Value
                .Should().Be("#rootElementId");

            var signedXml = new SignedXml(xmlDoc);
            signedXml.LoadXml(signature);
            signedXml.CheckSignature(TestCert, true).Should().BeTrue();
        }

        const string xmlString = "<xml a=\"b\">\n  <indented>content</indented>\n</xml>";
        readonly XmlDocument xmlDocument = XmlDocumentHelpers.FromString(xmlString);

        [TestMethod]
        public void XmlDocumentHelpers_FromString()
        {
            xmlDocument.OuterXml.Should().Be(xmlString);
        }

        [TestMethod]
        public void XmlDocumentHelpers_Remove_NullcheckAttribute()
        {
            ((XmlAttributeCollection)null).Invoking(
                a => a.Remove("attributeName"))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("attributes");
        }

        [TestMethod]
        public void XmlDocumentHelpers_Remove_NullcheckAttributeName()
        {
            xmlDocument.DocumentElement.Attributes.Invoking(
                a => a.Remove(attributeName: null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("attributeName");
        }

        [TestMethod]
        public void XmlDocumentHelpers_RemoveChild_NullcheckXmlElement()
        {
            new XmlDocument().DocumentElement.Invoking(
                e => e.RemoveChild("name", "ns"))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("xmlElement");
        }

        [TestMethod]
        public void XmlDocumentHelpers_RemoveChild_NullcheckName()
        {
            xmlDocument.DocumentElement.Invoking(
                e => e.RemoveChild(null, "ns"))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("name");
        }

        [TestMethod]
        public void XmlDocumentHelpers_RemoveChild_NullcheckNs()
        {
            xmlDocument.DocumentElement.Invoking(
                e => e.RemoveChild("name", null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("ns");
        }
    }
}