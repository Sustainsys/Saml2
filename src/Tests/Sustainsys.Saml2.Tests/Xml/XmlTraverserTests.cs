using Sustainsys.Saml2.Xml;
using Sustainsys.Saml2.Tests.Helpers;
using System;
using System.Linq;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Xml;

public class XmlTraverserTests
{
    private const string xml = "<root xmlns=\"urn:r\" xmlns:a=\"urn:a\" x=\"1\" a:x=\"2\" a:y=\"3\" a:z=\"4\" z=\"5\" " +
        "validTimeSpan=\"PT15M\" invalidTimeSpan=\"XYZ\" uri=\"urn:uri\"> <p/><q/>abc</root>";

    private readonly XmlDocument signedXmlDocument;

    private static XmlTraverser GetXmlTraverser(string xml = xml)
    {
        var xd = new XmlDocument()
        {
            PreserveWhitespace = true
        };

        xd.LoadXml(xml);

        return new(xd.DocumentElement!);
    }

    private XmlTraverser GetSignatureNode() 
    {
        var traverser = new XmlTraverser(signedXmlDocument!.DocumentElement!);

        var subject = traverser.GetChildren();

        subject.MoveNext().Should().BeTrue();

        return subject;
    }

    public XmlTraverserTests()
    {
        var xml = "<xml ID=\"id123\"/>";

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

        subject.MoveNext(true);

        subject.Invoking(s => s.ThrowOnErrors())
            .Should().Throw<SamlXmlException>()
            .Which.Errors.Single().Reason
            .Should().Be(ErrorReason.UnexpectedNamespace);
    }

    [Fact]
    public void ThrowsOnMultipleErrors()
    {
        var subject = GetXmlTraverser();

        subject.EnsureName("whatever", "something");

        subject.MoveNext(true);

        subject.Invoking(s => s.ThrowOnErrors())
            .Should().Throw<SamlXmlException>()
            .Which.Errors.Count().Should().Be(2);
    }

    [Fact]
    public void ThrowOnErrors_OnlyAllowedOnRoot()
    {
        var xml = "<r><a/></r>";

        var subject = GetXmlTraverser(xml).GetChildren();

        subject.Invoking(s => s.ThrowOnErrors())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*root traverser*");
    }

    [Fact]
    public void IgnoreSupressedError()
    {
        var subject = GetXmlTraverser();

        subject.EnsureName("whatever", "something");

        subject.MoveNext(true);

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

        subject.MoveNext(true);

        subject.Errors.Single(e => e.Reason == ErrorReason.UnexpectedLocalName).Ignore = true;

        subject.Invoking(s => s.ThrowOnErrors())
            .Should().Throw<SamlXmlException>()
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
        error.Node.Should().BeSameAs(subject.CurrentNode);
        error.StringValue.Should().Be("1");
    }

    [Fact]
    public void TraverseChildren()
    {
        var subject = GetXmlTraverser();
       
        var childElements = subject.GetChildren();

        childElements.MoveNext().Should().BeTrue();

        childElements.CurrentNode!.LocalName.Should().Be("p");

        childElements.MoveNext().Should().BeTrue();

        childElements.CurrentNode.LocalName.Should().Be("q");

        var grandChildElements = childElements.GetChildren();

        grandChildElements.MoveNext(true).Should().BeFalse();

        grandChildElements.CurrentNode.Should().BeNull();

        // Just check that we have no errors so far as we're soon checking error count = 1.
        subject.Errors.Should().HaveCount(0);

        childElements.MoveNext(true).Should().BeFalse();

        // We should how have hit the abc text node.
        subject.Errors.Should().HaveCount(1);
        subject.Errors.Last().Reason.Should().Be(ErrorReason.UnsupportedNodeType);
    }

    [Fact]
    public void ReadAndValidateOptionalSignature()
    {
        var subject = GetSignatureNode();

        subject.ReadAndValidateOptionalSignature(TestData.SingleSigningKey, SignedXmlHelperTests.allowedHashes, out var trustLevel)
            .Should().BeTrue();

        trustLevel.Should().Be(TestData.SigningKey.TrustLevel);
    }

    [Fact]
    public void ReadAndValidateOptionalSignature_WrongKey()
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

    [Fact]
    public void ReadAndValidateOptionalSignature_NotSignature()
    {
        var subject = GetXmlTraverser("<xml><a/></xml>").GetChildren();

        subject.MoveNext();

        subject.ReadAndValidateOptionalSignature(null, null, out var trustLevel).Should().BeFalse();

        trustLevel.Should().Be(TrustLevel.None);
    }

    [Fact]
    public void ReadAndValidateOptionalSignature_NoCurrentNode()
    {
        var subject = GetXmlTraverser("<xml/>").GetChildren();

        subject.MoveNext();

        subject.ReadAndValidateOptionalSignature(null, null, out var trustLevel).Should().BeFalse();

        trustLevel.Should().Be(TrustLevel.None);
    }

    [Fact]
    public void DetectSkippedChildren_NoChildAccess()
    {
        var xml = "<r><a><b/></a></r>";

        var subject = GetXmlTraverser(xml);

        var rChildren = subject.GetChildren();
        rChildren.MoveNext().Should().BeTrue();
        var a = rChildren.CurrentNode;

        rChildren.MoveNext(true).Should().BeFalse();

        subject.Errors.Should().HaveCount(1);

        var error = subject.Errors[0];
        error.Reason.Should().Be(ErrorReason.ExtraElements);
        error.Node.Should().BeSameAs(a);
        error.LocalName.Should().Be("a");
        error.Message.Should().Match("*not been processed*");
    }

    [Fact]
    public void DetectSkippedChildren_NotAllChildren()
    {
        var xml = "<r><a><b/></a></r>";

        var subject = GetXmlTraverser(xml);

        var rChildren = subject.GetChildren();
        rChildren.MoveNext().Should().BeTrue();
        var a = rChildren.CurrentNode;
        var aChildren = rChildren.GetChildren();
        
        aChildren.MoveNext().Should().BeTrue();
        aChildren.CurrentNode!.LocalName.Should().Be("b");

        rChildren.MoveNext(true).Should().BeFalse();

        subject.Errors.Should().HaveCount(1);
        
        var error = subject.Errors[0];
        error.Reason.Should().Be(ErrorReason.ExtraElements);
        error.Node.Should().BeSameAs(a);
        error.LocalName.Should().Be("a");
        error.Message.Should().Match("*not been processed*");
    }

    [Fact]
    public void ThrowOnErrors_DetectIfNotMovedOffRoot()
    {
        var xml = "<r/>";

        var subject = GetXmlTraverser(xml);

        subject.Invoking(s => s.ThrowOnErrors())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*move*root element*");
    }

    [Fact]

    public void IgnoreChildren()
    {
        var xml = "<r><a/></r>";

        var subject = GetXmlTraverser(xml);

        subject.IgnoreChildren();

        subject.MoveNext(true);

        subject.Errors.Should().HaveCount(0);
    }

    [Theory]
    [InlineData("<r/>", false)]
    [InlineData("<r><a/></r>", true)]
    [InlineData("<r>text</r>", false)]
    public void EnsureElement(string xml, bool expected)
    {
        var subject = GetXmlTraverser(xml);

        var children = subject.GetChildren();
        children.MoveNext(true).Should().Be(expected);

        children.EnsureElement().Should().Be(expected);

        if(!expected)
        { 
            // For the text node, there's first a text node error.
            subject.Errors.Should().NotBeEmpty();
            var e = subject.Errors.Last();

            e.Reason.Should().Be(ErrorReason.MissingElement);
            e.Node.Should().BeSameAs(subject.CurrentNode);
            e.LocalName.Should().Be("r");
            e.Message.Should().Match("*not*element*");
        }
    }

    [Fact]
    public void Skip()
    {
        var xml = "<r><a/></r>";

        var subject = GetXmlTraverser(xml);

        subject.GetChildren().Skip();

        subject.MoveNext(true);

        subject.Invoking(s => s.ThrowOnErrors())
            .Should().NotThrow();
    }
}
