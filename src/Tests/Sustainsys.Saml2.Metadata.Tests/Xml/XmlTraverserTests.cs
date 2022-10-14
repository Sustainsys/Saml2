using FluentAssertions;
using Sustainsys.Saml2.Metadata.Xml;
using Sustainsys.Saml2.Tests.Helpers;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Xunit;

namespace Sustainsys.Saml2.Metadata.Tests.Xml;

public class XmlTraverserTests
{
    private readonly XmlDocument xmlDocument;
    private readonly XmlDocument signedXmlDocument;

    private XmlTraverser GetXmlTraverser() => new(xmlDocument!.DocumentElement!);
    private XmlTraverser GetSignatureNode() 
    {
        var traverser = new XmlTraverser(signedXmlDocument!.DocumentElement!);

        var subject = traverser .GetChildren();

        subject.MoveNext().Should().BeTrue();

        return subject;
    }

    public XmlTraverserTests()
    {
        var xml = "<root xmlns=\"urn:r\" xmlns:a=\"urn:a\" x=\"1\" a:x=\"2\" a:y=\"3\" a:z=\"4\" z=\"5\" " +
            "validTimeSpan=\"PT15M\" invalidTimeSpan=\"XYZ\" uri=\"urn:uri\"> <p/><q/>abc</root>";

        xmlDocument = new XmlDocument()
        {
            PreserveWhitespace = true
        };
        xmlDocument.LoadXml(xml);

        xml = "<xml ID=\"id123\"/>";

        signedXmlDocument = new XmlDocument()
        {
            PreserveWhitespace = true
        };

        signedXmlDocument.LoadXml(xml);

        SignedXmlHelper.Sign(signedXmlDocument.DocumentElement!, TestData.Certificate);
    }

    [Theory]
    [InlineData("x", "1")]
    [InlineData("y", null)]
    [InlineData("z", "5")]
    public void GetAttribute(string localName, string expectedValue)
    {
        GetXmlTraverser().GetAttribute(localName).Should().Be(expectedValue);
    }

    private enum GetEnumAttributeEnum
    {
        Xyz = 42
    }

    [Fact]
    public void GetEnumAttribute()

    {
        GetXmlTraverser().GetEnumAttribute<GetEnumAttributeEnum>("invalidTimeSpan", true).Should().Be(GetEnumAttributeEnum.Xyz);

        GetXmlTraverser().GetEnumAttribute<GetEnumAttributeEnum>("validTimeSpan", true).Should().Be(null);
    }

    [Theory]
    [InlineData("urn:r", "root")]
    [InlineData("urn:X", "root", ErrorReason.UnexpectedNamespace)]
    [InlineData("urn:r", "X", ErrorReason.UnexpectedLocalName)]
    [InlineData("urn:X", "X", ErrorReason.UnexpectedLocalName, ErrorReason.UnexpectedNamespace)]
    public void EnsureName(string ns, string localName, params ErrorReason[] errorReasons)
    {
        var subject = GetXmlTraverser();

        var actual = subject.EnsureName(ns, localName);

        actual.Should().Be(errorReasons.Length == 0);

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

        foreach (var error in subject.Errors)
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

    [Fact]
    public void GetTimeSpanAttribute()
    {
        GetXmlTraverser().GetTimeSpanAttribute("validTimeSpan").Should().Be(TimeSpan.FromMinutes(15));
    }

    [Fact]
    public void GetTimeSpanAttribute_ParseError()
    {
        var subject = GetXmlTraverser();

        var actual = subject.GetTimeSpanAttribute("invalidTimeSpan");

        actual.HasValue.Should().BeFalse();

        subject.Errors.Should().HaveCount(1);
        subject.Errors.Single().Reason.Should().Be(ErrorReason.ConversionFailed);
        subject.Errors.Single().StringValue.Should().Be("XYZ");
    }

    [Fact]
    public void GetRequiredAbsoluteUriAttribute()
    {
        GetXmlTraverser().GetRequiredAbsoluteUriAttribute("uri").Should().Be("urn:uri");
    }

    [Fact]
    public void GetRequiredAbsoluteUriAttribute_NotAbsoluteUri()
    {
        var subject = GetXmlTraverser();

        subject.GetRequiredAbsoluteUriAttribute("x").Should().Be("1");

        subject.Errors.Should().HaveCount(1);
        var error = subject.Errors.Single();

        error.Reason.Should().Be(ErrorReason.NotAbsoluteUri);
        error.LocalName.Should().Be("x");
        error.Node.Should().BeSameAs(xmlDocument.DocumentElement);
        error.StringValue.Should().Be("1");
    }

    [Fact]
    public void TraverseChildren()
    {
        var subject = GetXmlTraverser();

        var parentNode = subject.CurrentNode;

        var childElements = subject.GetChildren();

        childElements.MoveNext().Should().BeTrue();

        childElements.CurrentNode!.LocalName.Should().Be("p");

        childElements.MoveNext().Should().BeTrue();

        childElements.CurrentNode.LocalName.Should().Be("q");

        var grandChildElements = childElements.GetChildren();

        grandChildElements.MoveNext(true).Should().BeFalse();

        grandChildElements.Invoking(s => s.CurrentNode).Should().Throw<InvalidOperationException>();

        // Just check that we have no errors so far as we're soon checking error count = 1.
        subject.Errors.Should().HaveCount(0);

        childElements.MoveNext(true).Should().BeFalse();

        // We should how have hit the abc text node.
        subject.Errors.Should().HaveCount(1);
        subject.Errors.Last().Reason.Should().Be(ErrorReason.UnsupportedNodeType);
    }

    [Fact]
    public void ValidateSignature()
    {
        var subject = GetSignatureNode();

        subject.ReadAndValidateOptionalSignature(TestData.SingleSigningKey, SignedXmlHelperTests.allowedHashes, out var trustLevel)
            .Should().BeTrue();

        trustLevel.Should().Be(TestData.SigningKey.TrustLevel);
    }

    [Fact]
    public void ValidateSignatureWrongKey()
    {
        var subject = GetSignatureNode();

        subject.ReadAndValidateOptionalSignature(TestData.SingleSigningKey2, SignedXmlHelperTests.allowedHashes, out var trustLevel)
            .Should().BeTrue();

        trustLevel.Should().Be(TrustLevel.None);

        subject.Errors.Should().HaveCount(1);

        var error = subject.Errors[0];
        
        error.Message.Should().Match("*contained key*not*trusted*");
        error.Reason.Should().Be(ErrorReason.SignatureFailure);
    }
}
