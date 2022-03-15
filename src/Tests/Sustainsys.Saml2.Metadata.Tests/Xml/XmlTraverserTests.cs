using FluentAssertions;
using Sustainsys.Saml2.Metadata.Xml;
using System.Xml;
using Xunit;

namespace Sustainsys.Saml2.Metadata.Tests.Xml;

public class XmlTraverserTests
{
    private XmlTraverser xmlTraverser;

    public XmlTraverserTests()
    {
        var xml = "<root xmlns=\"urn:r\" xmlns:a=\"urn:a\" x=\"1\" a:x=\"2\" a:y=\"3\"/>";

        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);

        xmlTraverser = new XmlTraverser(xmlDocument.DocumentElement!);
    }

    [Theory]
    [InlineData("x", "1")]
    [InlineData("y", null)]
    public void GetAttribute(string localName, string expectedValue)
    {
        xmlTraverser.GetAttribute(localName).Should().Be(expectedValue);
    }
}
