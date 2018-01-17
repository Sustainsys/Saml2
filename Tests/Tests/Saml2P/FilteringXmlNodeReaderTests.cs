using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Sustainsys.Saml2.Saml2P;
using FluentAssertions;

namespace Sustainsys.Saml2.Tests.Saml2P
{
    [TestClass]
    public class FilteringXmlNodeReaderTests
    {
        [TestMethod]
        public void FilteringXmlNodeReader_RemovesFilteredOutNode_PreserveWhiteSpace()
        {
            FilteringXmlNodeReader_RemovesFilteredOutNode(true);
        }

        [TestMethod]
        public void FilteringXmlNodeReader_RemovesFilteredOutNode_NoPreserveWhiteSpace()
        {
            FilteringXmlNodeReader_RemovesFilteredOutNode(false);
        }

        void FilteringXmlNodeReader_RemovesFilteredOutNode(bool preserveWhiteSpace)
        {
            var xmlData =
            @"<a:root xmlns:a=""urn:a"" xmlns:b=""urn:b"">
                <a:preserveChild>Preserve</a:preserveChild>
                <a:removeChild>
                    <b:removeThisToo>Remove this!</b:removeThisToo>
                </a:removeChild>
                <b:removeChild>Just kidding, this should be kept.</b:removeChild>
            </a:root>";

            var xmlDoc = XmlHelpers.XmlDocumentFromString(xmlData);

            var subject = new FilteringXmlNodeReader("urn:a", "removeChild", xmlDoc.DocumentElement);

            subject.ReadStartElement("root", "urn:a");
            subject.ReadStartElement("preserveChild", "urn:a");
            subject.Skip();
            subject.ReadEndElement();
            subject.ReadStartElement("removeChild", "urn:b");
            subject.Skip();
            subject.ReadEndElement();

            subject.MoveToContent();
            subject.NodeType.Should().Be(XmlNodeType.EndElement);
            subject.Name.Should().Be("a:root");
        }
    }
}
