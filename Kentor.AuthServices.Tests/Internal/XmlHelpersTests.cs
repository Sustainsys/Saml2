using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using FluentAssertions;
using System.Xml;
using Kentor.AuthServices.Internal;
using Kentor.AuthServices.Tests.Helpers;

namespace Kentor.AuthServices.Tests.Internal
{
    [TestClass]
    public class XmlHelpersTests
    {
        [TestMethod]
        public void XmlHelpers_AddAttributeIfNotNullOrEmpty_Adds()
        {
            var e = new XElement("xml");

            e.AddAttributeIfNotNullOrEmpty("attribute", "value");

            e.Attribute("attribute").Should().NotBeNull().And.Subject.Value
                .Should().Be("value");
        }

        [TestMethod]
        public void XmlHelpers_AddAttributeIfNotNullOrEmpty_IgnoresEmpty()
        {
            var e = new XElement("xml");

            e.AddAttributeIfNotNullOrEmpty("attribute", "");

            e.Attribute("attribute").Should().BeNull();
        }

        [TestMethod]
        public void XmlHelpers_AddAttributeIfNotNullOrEmpty_HandlesNamespace()
        {
            var e = new XElement("xml");

            var ns = XNamespace.Get("someNamespace");

            e.AddAttributeIfNotNullOrEmpty(ns + "attribute", "");

            e.Attribute(ns + "attribute").Should().BeNull();
        }

        [TestMethod]
        public void XmlHelpers_AddAttributeIfNotNullOrEmpty_IgnoresNull()
        {
            var e = new XElement("xml");

            e.AddAttributeIfNotNullOrEmpty("attribute", null);

            e.Attribute("attribute").Should().BeNull();
        }

        [TestMethod]
        public void XmlHelpers_AddAttributeIfNotNullOrEmpty_HandlesUri()
        {
            var e = new XElement("xml");

            string uri = "http://some.example.com/";
            e.AddAttributeIfNotNullOrEmpty("attribute", new Uri(uri));

            e.Attribute("attribute").Should().NotBeNull().And.Subject.Value.Should().Be(uri);
        }

        class EmptyToString { public override string ToString() { return string.Empty; } }

        [TestMethod]
        public void XmlHelpers_AddAttributeIfNotNullOrEmpty_IgnoresObjectWithEmptyToString()
        {
            var e = new XElement("xml");

            e.AddAttributeIfNotNullOrEmpty("attribute", new EmptyToString());

            e.Attribute("attribute").Should().BeNull();
        }

        [TestMethod]
        public void XmlHelpers_AddAttributeIfNotNullOrEmtpy_TimeSpanSerializedCorrectly()
        {
            // It might be tempting in the implementation to call value.ToString()
            // instead of passing in the value. That would make types that have
            // special XML Serialization formats fail. This test ensures that
            // nobody takes that shortcut without handling the special cases.

            var e = new XElement("xml");

            e.AddAttributeIfNotNullOrEmpty("attribute", new TimeSpan(2, 17, 32));

            e.Attribute("attribute").Should().NotBeNull().And.Subject.Value.Should().Be("PT2H17M32S");
        }

        [TestMethod]
        public void XmlHelpers_GetValueIfNotNull_NullOnNull()
        {
            XmlAttribute x = null;
            x.GetValueIfNotNull().Should().BeNull();
        }

        [TestMethod]
        public void XmlHelpers_GetValueIfNotNull_ValueOnNotNull()
        {
            var xd = new XmlDocument();
            var a = xd.CreateAttribute("someAttribute");
            a.Value = "SomeValue";

            a.GetValueIfNotNull().Should().Be("SomeValue");
        }

        [TestMethod]
        public void XmlHelpers_GetTrimmedTextIfNotNull_ValueOnNotNull()
        {
            var xd = new XmlDocument();
            var e = xd.CreateElement("someElement");
            e.InnerText = "\r\n     Some Text";

            e.GetTrimmedTextIfNotNull().Should().Be("Some Text");
        }

        [TestMethod]
        public void XmlHelpers_GetTrimmedTextIfNotNull_NullOnNull()
        {
            XmlElement e = null;

            e.GetTrimmedTextIfNotNull().Should().BeNull();
        }

        [TestMethod]
        public void XmlHelpers_PrettyPrint_Nullcheck()
        {
            Action a = () => ((XmlElement)null).PrettyPrint();

            a.ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("xml");
        }

        [TestMethod]
        public void XmlHelpers_PrettyPrint()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<a><b>c</b></a>");

            var result = xmlDoc.DocumentElement.PrettyPrint();

            var parsed = XmlHelpers.FromString(result);

            var expected = "<a>\r\n  <b>c</b>\r\n</a>";

            parsed.OuterXml.Should().Be(expected);
            // Don't change semantics.
            parsed.DocumentElement.Should().BeEquivalentTo(xmlDoc.DocumentElement);
        }
    }
}
