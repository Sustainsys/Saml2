using Sustainsys.Saml2.Tests.Helpers;
using Sustainsys.Saml2.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Xml;
public class XmlTraverserTests
{
    private const string xml = "<root xmlns=\"urn:r\" xmlns:a=\"urn:a\" x=\"1\" a:x=\"2\" a:y=\"3\" a:z=\"4\" five=\"5\" " + "validTimeSpan=\"PT15M\" xyz=\"XYZ\" def=\"DEF\" uri=\"urn:uri\" bool=\"true\"><p/><q/>abc</root>";
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

    [Test]
    [Arguments("x", "1")]
    [Arguments("y", null)]
    [Arguments("five", "5")]
    public void GetAttribute(string localName, string? expectedValue)
    {
        GetXmlTraverser().GetAttribute(localName).Should().Be(expectedValue);
    }

    [Test]
    [Arguments("urn:r", "root")]
    [Arguments("urn:X", "root", ErrorReason.UnexpectedNamespace)]
    [Arguments("urn:r", "X", ErrorReason.UnexpectedLocalName)]
    [Arguments("urn:X", "X", ErrorReason.UnexpectedLocalName, ErrorReason.UnexpectedNamespace)]
    public void EnsureName(string ns, string localName, params ErrorReason[] errorReasons)
    {
        var subject = GetXmlTraverser();
        var actual = subject.EnsureName(localName, ns);
        actual.Should().Be(errorReasons.Length == 0);
        subject.Errors.Select(e => e.Reason).Should().BeEquivalentTo(errorReasons);
    }

    [Test]
    public void ThrowsOnError()
    {
        var subject = GetXmlTraverser();
        subject.EnsureName("root", "whatever");
        subject.IgnoreChildren();
        subject.MoveNext(true);
        subject.Invoking(s => s.ThrowOnErrors()).Should().Throw<SamlXmlException>().Which.Errors.Single().Reason.Should().Be(ErrorReason.UnexpectedNamespace);
    }

    [Test]
    public void ThrowsOnMultipleErrors()
    {
        var subject = GetXmlTraverser();
        subject.EnsureName("something", "whatever");
        subject.MoveNext(true);
        // 3 errors: local name, namespace and children not processed.
        subject.Invoking(s => s.ThrowOnErrors()).Should().Throw<SamlXmlException>().Which.Errors.Count().Should().Be(3);
    }

    [Test]
    public void ThrowOnErrors_OnlyAllowedOnRoot()
    {
        var xml = "<r><a/></r>";
        var subject = GetXmlTraverser(xml).GetChildren();
        subject.Invoking(s => s.ThrowOnErrors()).Should().Throw<InvalidOperationException>().WithMessage("*root traverser*");
    }

    [Test]
    public void IgnoreSupressedError()
    {
        var subject = GetXmlTraverser();
        subject.EnsureName("something", "whatever");
        subject.MoveNext(true);
        foreach (var error in subject.Errors)
        {
            error.Ignore = true;
        }

        subject.Invoking(s => s.ThrowOnErrors()).Should().NotThrow();
    }

    [Test]
    public void HandlesMixedSupression()
    {
        var subject = GetXmlTraverser();
        subject.EnsureName("something", "whatever");
        subject.IgnoreChildren();
        subject.MoveNext(true);
        subject.Errors.Single(e => e.Reason == ErrorReason.UnexpectedLocalName).Ignore = true;
        subject.Invoking(s => s.ThrowOnErrors()).Should().Throw<SamlXmlException>() // Message should only contain the non-ignored message.
        .WithMessage("Unexpected namespace \"urn:r\" for local name \"root\", expected \"whatever\".") // But the collection should contain everything.
        .Which.Errors.Count().Should().Be(2);
    }

    private enum GetEnumAttributeEnum
    {
        Def = 42
    }

    [Test]
    public void GetEnumAttribute()
    {
        GetXmlTraverser().GetEnumAttribute<GetEnumAttributeEnum>("def", true).Should().Be(GetEnumAttributeEnum.Def);
    }

    [Test]
    public void GetEnumAttribute_Missing()
    {
        GetXmlTraverser().GetEnumAttribute<GetEnumAttributeEnum>("notexisting", true).Should().BeNull();
    }

    [Test]
    public void GetEnumAttribute_ParseError()
    {
        var subject = GetXmlTraverser();
        var actual = subject.GetEnumAttribute<GetEnumAttributeEnum>("xyz", true);
        actual.Should().BeNull();
        ValidateParseError(subject);
    }

    [Test]
    public void GetTimeSpanAttribute()
    {
        GetXmlTraverser().GetTimeSpanAttribute("validTimeSpan").Should().Be(TimeSpan.FromMinutes(15));
    }

    [Test]
    public void GetTimeSpanAttribute_ParseError()
    {
        var subject = GetXmlTraverser();
        var actual = subject.GetTimeSpanAttribute("xyz");
        actual.Should().BeNull();
        ValidateParseError(subject);
    }

    private static void ValidateParseError(XmlTraverser subject)
    {
        subject.Errors.Should().HaveCount(1);
        var error = subject.Errors.Single();
        error.Reason.Should().Be(ErrorReason.ConversionFailed);
        error.StringValue.Should().Be("XYZ");
        error.LocalName.Should().Be("xyz");
        error.Node.Should().BeSameAs(subject.CurrentNode);
    }

    [Test]
    public void GetRequiredAbsoluteUriAttribute()
    {
        GetXmlTraverser().GetRequiredAbsoluteUriAttribute("uri").Should().Be("urn:uri");
    }

    [Test]
    public void GetRequiredAbsoluteUriAttribute_Missing()
    {
        var subject = GetXmlTraverser();
        subject.GetRequiredAbsoluteUriAttribute("notExisting").Should().BeNull();
        ValidateMissing(subject);
    }

    [Test]
    [Arguments("<a></a>", "", ErrorReason.EmptyElement)]
    [Arguments("<a>https://example.com</a>", "https://example.com", null)]
    [Arguments("<a>example.com</a>", "example.com", ErrorReason.NotAbsoluteUri)]
    public void GetAbsoluteUriContents(string xml, string expectedValue, ErrorReason? expectedError)
    {
        var subject = GetXmlTraverser(xml);
        var actualValue = subject.GetAbsoluteUriContents();
        actualValue.Should().Be(expectedValue);
        if (expectedError == null)
        {
            subject.Errors.Count().Should().Be(0);
        }
        else
        {
            subject.Errors.Count().Should().Be(1);
            subject.Errors[0].Reason.Should().Be(expectedError);
        }
    }

    private static void ValidateMissing(XmlTraverser subject)
    {
        subject.Errors.Count.Should().Be(1);
        var error = subject.Errors.Single();
        error.Reason.Should().Be(ErrorReason.MissingAttribute);
        error.StringValue.Should().BeNull();
        error.LocalName.Should().Be("notExisting");
        error.Node.Should().BeSameAs(subject.CurrentNode);
    }

    [Test]
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

    [Test]
    public void GetBoolAttribute()
    {
        GetXmlTraverser().GetBoolAttribute("bool").Should().BeTrue();
    }

    [Test]
    public void GetBoolAttribute_Missing()
    {
        GetXmlTraverser().GetBoolAttribute("notExisting").Should().BeNull();
    }

    [Test]
    public void GetBoolAttribute_ParseError()
    {
        var subject = GetXmlTraverser();
        var actual = subject.GetBoolAttribute("xyz");
        actual.Should().BeNull();
        ValidateParseError(subject);
    }

    [Test]
    public void GetIntAttribute()
    {
        GetXmlTraverser().GetIntAttribute("five").Should().Be(5);
    }

    [Test]
    public void GetIntAttribute_Missing()
    {
        GetXmlTraverser().GetIntAttribute("notExisting").Should().BeNull();
    }

    [Test]
    public void GetIntAttribute_ParseError()
    {
        var subject = GetXmlTraverser();
        var actual = subject.GetIntAttribute("xyz");
        actual.HasValue.Should().BeFalse();
        ValidateParseError(subject);
    }

    [Test]
    public void GetRequiredIntAttribute()
    {
        GetXmlTraverser().GetRequiredIntAttribute("five").Should().Be(5);
    }

    [Test]
    public void GetRequiredIntAttribute_Missing()
    {
        var subject = GetXmlTraverser();
        subject.GetRequiredIntAttribute("notExisting").Should().Be(0);
        ValidateMissing(subject);
    }

    [Test]
    public void GetRequiredIntAttribute_ParseError()
    {
        var subject = GetXmlTraverser();
        var actual = subject.GetRequiredIntAttribute("xyz");
        actual.Should().Be(default);
        ValidateParseError(subject);
    }

    [Test]
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

    [Test]
    public void ReadAndValidateOptionalSignature()
    {
        var subject = GetSignatureNode();
        subject.ReadAndValidateOptionalSignature(TestData.SingleSigningKey, SignedXmlHelperTests.allowedHashes).Should().BeTrue();
        subject.TrustLevel.Should().Be(TestData.SigningKey.TrustLevel);
    }

    [Test]
    public void ReadAndValidateOptionalSignature_WrongKey()
    {
        var subject = GetSignatureNode();
        subject.ReadAndValidateOptionalSignature(TestData.SingleSigningKey2, SignedXmlHelperTests.allowedHashes).Should().BeTrue();
        subject.TrustLevel.Should().Be(TrustLevel.None);
        subject.Errors.Should().HaveCount(1);
        var error = subject.Errors[0];
        error.Message.Should().Match("*contained key*not*trusted*");
        error.Reason.Should().Be(ErrorReason.SignatureFailure);
    }

    [Test]
    public void ReadAndValidateOptionalSignature_NotSignature()
    {
        var subject = GetXmlTraverser("<xml><a/></xml>").GetChildren();
        subject.MoveNext();
        subject.ReadAndValidateOptionalSignature(null, null).Should().BeFalse();
        subject.TrustLevel.Should().Be(TrustLevel.None);
    }

    [Test]
    public void ReadAndValidateOptionalSignature_NoCurrentNode()
    {
        var subject = GetXmlTraverser("<xml/>").GetChildren();
        subject.MoveNext();
        subject.ReadAndValidateOptionalSignature(null, null).Should().BeFalse();
        subject.TrustLevel.Should().Be(TrustLevel.None);
    }

    [Test]
    [Arguments(null, TrustLevel.TLS)]
    [Arguments(TrustLevel.Http, TrustLevel.TLS)]
    [Arguments(TrustLevel.ConfiguredKey, TrustLevel.ConfiguredKey)]
    public void TrustLevel_Inheritance(TrustLevel? childTrustLevel, TrustLevel expected)
    {
        var subject = GetXmlTraverser("<root ID=\"r1\"><x/><child ID=\"c1\"/></root>");
        if (childTrustLevel.HasValue)
        {
            var childElement = (XmlElement)subject.RootNode!.SelectSingleNode("/root/child")!;
            childElement.Sign(TestData.Certificate);
        }

        ((XmlElement)subject.CurrentNode!).Sign(TestData.Certificate, subject.RootNode!.SelectSingleNode("/root/x")!);
        var children = subject.GetChildren();
        children.MoveNext().Should().BeTrue(); // Step to X
        children.MoveNext().Should().BeTrue(); // Step to signature
        // Read and validate the signature of root as TrustLevel.TLS.
        SigningKey[] tlsKey = [new()
        {
            Certificate = TestData.Certificate,
            TrustLevel = TrustLevel.TLS
        }

        ];
        children.ReadAndValidateOptionalSignature(tlsKey, ["sha256"]).Should().BeTrue();
        children.TrustLevel.Should().Be(TrustLevel.TLS);
        children.MoveNext();
        var grandChildren = children.GetChildren();
        // Now read and validate the signature of child.
        if (grandChildren.MoveNext(true) && childTrustLevel.HasValue)
        {
            SigningKey[] trustedKeys = [new()
            {
                Certificate = TestData.Certificate,
                TrustLevel = childTrustLevel.Value
            }

            ];
            grandChildren.ReadAndValidateOptionalSignature(trustedKeys, ["sha256"]).Should().BeTrue();
        }

        // Finally validate that the TrustLevel of grandChilden is correct with regards to inheritance.
        grandChildren.TrustLevel.Should().Be(expected);
    }

    [Test]
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

    [Test]
    public void DetectSkippedChildren_NotAllChildrenNested()
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

    [Test]
    public void DetectSkippedChildren_NotAllChildren()
    {
        var xml = "<r><a/><b/><c/></r>";
        var subject = GetXmlTraverser(xml);
        var children = subject.GetChildren();
        children.MoveNext().Should().BeTrue();
        var a = children.CurrentNode!;
        subject.MoveNext(true).Should().BeFalse();
        subject.Errors.Should().HaveCount(1);
        var error = subject.Errors[0];
        error.Reason.Should().Be(ErrorReason.ExtraElements);
        error.Node.Should().BeSameAs(a.ParentNode);
        error.LocalName.Should().Be("r");
        error.Message.Should().Match("*not been processed*");
    }

    [Test]
    public void ThrowOnErrors_DetectIfNotMovedOffRoot()
    {
        var xml = "<r/>";
        var subject = GetXmlTraverser(xml);
        subject.Invoking(s => s.ThrowOnErrors()).Should().Throw<InvalidOperationException>().WithMessage("*move*root element*");
    }

    [Test]
    public void IgnoreChildren()
    {
        var xml = "<r><a/></r>";
        var subject = GetXmlTraverser(xml);
        subject.IgnoreChildren();
        subject.MoveNext(true);
        subject.Errors.Should().HaveCount(0);
    }

    [Test]
    public void Skip()
    {
        var xml = "<r><a/></r>";
        var subject = GetXmlTraverser(xml);
        subject.GetChildren().Skip();
        subject.MoveNext(true);
        subject.Invoking(s => s.ThrowOnErrors()).Should().NotThrow();
    }
}