using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Kentor.AuthServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    using System.Globalization;
    using System.Threading;

    [TestClass]
    public class XmlDocumentExtensionsTests
    {
        public static readonly X509Certificate2 TestCert = new X509Certificate2("Kentor.AuthServices.Tests.pfx");

        [TestInitialize]
        public void MyTestInitialize()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        [TestMethod]
        public void XmlDocumentExtensions_Sign_Nullcheck_xmlDocument()
        {
            XmlDocument xd = null;
            Action a = () => xd.Sign(TestCert);

            a.ShouldThrow<ArgumentNullException>().WithMessage(
                "Value cannot be null.\r\nParameter name: xmlDocument");
        }

        [TestMethod]
        public void XmlDocumentExtensions_Sign_Nullcheck_Cert()
        {
            XmlDocument xd = new XmlDocument();
            Action a = () => xd.Sign(null);

            a.ShouldThrow<ArgumentNullException>().WithMessage(
                "Value cannot be null.\r\nParameter name: cert");
        }

        [TestMethod]
        public void XmlDocumentExtensions_Sign()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<root ID=\"roolElementId\"><content>Some Content</content></root>");

            xmlDoc.Sign(TestCert);

            var signature = xmlDoc.DocumentElement["Signature", SignedXml.XmlDsigNamespaceUrl];

            var signedXml = new SignedXml(xmlDoc);

            signedXml.LoadXml(signature);

            signedXml.CheckSignature(TestCert, true).Should().BeTrue();
        }
    }
}
