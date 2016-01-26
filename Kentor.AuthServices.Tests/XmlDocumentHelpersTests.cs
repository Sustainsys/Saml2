using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using FluentAssertions;
using Kentor.AuthServices.Tests.Helpers;
using Kentor.AuthServices.Exceptions;

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
        public void XmlDocumentHelpers_Sign_Nullcheck_xmlElement()
        {
            ((XmlElement)null).Invoking(
                x => x.Sign(SignedXmlHelper.TestCert, true))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("xmlElement");
        }

        [TestMethod]
        public void XmlDocumentHelpers_Sign_Nullcheck_Cert()
        {
            xmlDocument.DocumentElement.Invoking(
                x => x.Sign(null, false))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("cert");
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

        [TestMethod]
        public void XmlDocumentHelpers_IsSignedBy_NullcheckXmlElement()
        {
            ((XmlElement)null).Invoking(
                x => x.IsSignedBy(SignedXmlHelper.TestCert))
                .ShouldThrow<ArgumentNullException>("xmlElement");
        }

        [TestMethod]
        public void XmlDocumentHelpers_IsSignedBy_NullcheckCertificate()
        {
            xmlDocument.DocumentElement.Invoking(
                x => x.IsSignedBy(null))
                .ShouldThrow<ArgumentNullException>("certificate");
        }

        [TestMethod]
        public void XmlDocumentHelpers_IsSignedBy()
        {
            var xml = "<xml ID=\"someID\"><content>text</content></xml>";
            var xmlDoc = XmlDocumentHelpers.FromString(xml);
            xmlDoc.Sign(SignedXmlHelper.TestCert);

            xmlDoc.DocumentElement.IsSignedBy(SignedXmlHelper.TestCert).Should().BeTrue();
        }

        [TestMethod]
        public void XmlDocumentHelpers_IsSignedBy_FalseOnWrongCert()
        {
            var xml = "<xml ID=\"someID\"><content>text</content></xml>";
            var xmlDoc = XmlDocumentHelpers.FromString(xml);
            xmlDoc.Sign(SignedXmlHelper.TestCert);

            xmlDoc.DocumentElement.IsSignedBy(SignedXmlHelper.TestCert2).Should().BeFalse();
        }

        [TestMethod]
        public void XmlDocumentHelpers_IsSignedBy_FalseOnTamperedData()
        {
            var xml = "<xml ID=\"someID\"><content>text</content></xml>";
            var xmlDoc = XmlDocumentHelpers.FromString(xml);
            xmlDoc.Sign(SignedXmlHelper.TestCert);

            xmlDoc.DocumentElement["content"].InnerText = "changedText";

            xmlDoc.DocumentElement.IsSignedBy(SignedXmlHelper.TestCert).Should().BeFalse();
        }

        [TestMethod]
        public void XmlDocumentHelpers_IsSignedBy_TrowsOnSignatureWrapping()
        {
            var xml = "<xml ID=\"someID\"><content ID=\"content1\">text</content>"
                + "<injected>other text</injected></xml>";
            var xmlDoc = XmlDocumentHelpers.FromString(xml);

            xmlDoc.DocumentElement["content"].Sign(SignedXmlHelper.TestCert, false);

            // An XML wrapping attack is created by taking a legitimate signature
            // and putting it in another element. If the reference of the signature
            // is not properly checked, the element containing the signature
            // is incorrectly trusted.
            var signatureNode = xmlDoc.DocumentElement["content"]["Signature", SignedXml.XmlDsigNamespaceUrl];
            xmlDoc.DocumentElement["content"].RemoveChild(signatureNode);
            xmlDoc.DocumentElement["injected"].AppendChild(signatureNode);

            xmlDoc.DocumentElement["injected"].Invoking(
                x => x.IsSignedBy(SignedXmlHelper.TestCert))
                .ShouldThrow<InvalidSignatureException>()
                .And.Message.Should().Be("Incorrect reference on Xml signature. The reference must be to the root element of the element containing the signature.");
        }

        [TestMethod]
        public void XmlDocumentHelpers_IsSignedBy_FalseOnMissingSignature()
        {
            var xml = "<xml ID=\"someID\"><content>text</content></xml>";
            var xmlDoc = XmlDocumentHelpers.FromString(xml);

            xmlDoc.DocumentElement.IsSignedBy(SignedXmlHelper.TestCert).Should().BeFalse();
        }
    }
}