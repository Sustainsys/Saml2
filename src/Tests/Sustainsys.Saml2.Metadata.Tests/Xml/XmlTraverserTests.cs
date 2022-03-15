using FluentAssertions;
using Sustainsys.Saml2.Metadata.Xml;
using System.Linq;
using System.Xml;
using Xunit;

namespace Sustainsys.Saml2.Metadata.Tests.Xml;

public class XmlTraverserTests
{
    private XmlDocument xmlDocument;

    private XmlTraverser GetXmlTraverser() => new XmlTraverser(xmlDocument!.DocumentElement!);

    public XmlTraverserTests()
    {
        var xml = "<root xmlns=\"urn:r\" xmlns:a=\"urn:a\" x=\"1\" a:x=\"2\" a:y=\"3\" a:z=\"4\" z=\"5\"/>";

        xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
    }

    [Theory]
    [InlineData("x", "1")]
    [InlineData("y", null)]
    [InlineData("z", "5")]
    public void GetAttribute(string localName, string expectedValue)
    {
        GetXmlTraverser().GetAttribute(localName).Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("urn:r", "root")]
    [InlineData("urn:X", "root", ErrorReason.UnexpectedNamespace)]
    [InlineData("urn:r", "X", ErrorReason.UnexpectedLocalName)]
    [InlineData("urn:X", "X", ErrorReason.UnexpectedLocalName, ErrorReason.UnexpectedNamespace)]
    public void EnsureName(string ns, string localName, params ErrorReason[] errorReasons)
    {
        var subject = GetXmlTraverser();

        subject.EnsureName(ns, localName);

        subject.Errors.Select(e => e.Reason).Should().BeEquivalentTo(errorReasons);
    }

    [Fact]
    public void ThrowsOnError()
    {
        var subject = GetXmlTraverser();

        subject.EnsureName("whatever", "root");

        subject.Invoking(s => s.ThrowOnErrors())
            .Should().Throw<Saml2XmlException>()
            .Which.Errors.Single().Reason
            .Should().Be(ErrorReason.UnexpectedNamespace);
    }

    [Fact]
    public void ThrowsOnMultipleErrors()
    {
        var subject = GetXmlTraverser();

        subject.EnsureName("whatever", "something");

        subject.Invoking(s => s.ThrowOnErrors())
            .Should().Throw<Saml2XmlException>()
            .Which.Errors.Count().Should().Be(2);
    }

    [Fact]
    public void IgnoreSupressedError()
    {
        var subject = GetXmlTraverser();

        subject.EnsureName("whatever", "something");

        foreach(var error in subject.Errors)
        {
            error.Ignore = true;
        }

        subject.Invoking(s => s.ThrowOnErrors())
            .Should().NotThrow();
    }

    [Fact]
    public void HandlesMixedSupression()
    {
        var subject = GetXmlTraverser();

        subject.EnsureName("whatever", "something");

        subject.Errors.Single(e => e.Reason == ErrorReason.UnexpectedLocalName).Ignore = true;

        subject.Invoking(s => s.ThrowOnErrors())
            .Should().Throw<Saml2XmlException>()
            // Message should only contain the non-ignored message.
            .WithMessage("Unexpected namespace \"urn:r\" for local name \"root\", expected \"whatever\".")
            // But the collection should contain everything.
            .Which.Errors.Count().Should().Be(2);
    }
}
