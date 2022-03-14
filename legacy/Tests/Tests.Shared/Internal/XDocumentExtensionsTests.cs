using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Sustainsys.Saml2.Tests.Internal
{
    [TestClass]
    public class XDocumentExtensionsTests
    {
        [TestMethod]
        public void XDocumentExtensions_ToStringWithXmlDeclaration_NullDeclaration()
        {
            var xd = new XDocument(new XElement("xml"));

            xd.ToStringWithXmlDeclaration().Should().Be("<xml />");
        }

        [TestMethod]
        public void XDocumentExtensions_ToStringWithXmlDeclaration_WithDeclaration()
        {
            var xd = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement("xml"));

            var expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<xml />";

            var actual = xd.ToStringWithXmlDeclaration();

            expected.Should().Be(actual);
        }
    }
}
