﻿using Sustainsys.Saml2.Xml;
using Sustainsys.Saml2.Tests.Helpers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Xml;

public class SignedXmlHelperTests
{
    public static readonly string[] allowedHashes = { "sha256" };

    private static XmlDocument CreateSignedDocument()
    {
        var xml = "<xml ID=\"id123\"/>";

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        xmlDoc.DocumentElement!.Sign(TestData.Certificate);

        return xmlDoc;
    }

    private static XmlElement GetSignatureElement()
    {
        var xmlDocument = CreateSignedDocument();

        return (XmlElement)xmlDocument.DocumentElement!.FirstChild!;
    }

    [Fact]
    public void Sign()
    {
        var xmlDoc = CreateSignedDocument();

        var signature = xmlDoc.DocumentElement!["Signature", SignedXml.XmlDsigNamespaceUrl]!;

        signature["SignedInfo", SignedXml.XmlDsigNamespaceUrl]!
            ["Reference", SignedXml.XmlDsigNamespaceUrl]!.Attributes["URI"]!.Value
            .Should().Be("#id123");

        // Default algorithms in SignedXml are rsa-sha1 and sha1. Default
        // in Saml2 are rsa-sha256 and sha256. Ensure Saml2
        // defaults are used and not SignedXml/.NET defaults.
        signature["SignedInfo"]!["SignatureMethod"]!.GetAttribute("Algorithm")
            .Should().Be(SignedXml.XmlDsigRSASHA256Url);
        signature["SignedInfo"]!["Reference"]!["DigestMethod"]!.GetAttribute("Algorithm")
            .Should().Be(SignedXml.XmlDsigSHA256Url);

        var signedXml = new SignedXml(xmlDoc);
        signedXml.LoadXml(signature);
        signedXml.CheckSignature(TestData.Certificate, true).Should().BeTrue();
    }

    [Fact]
    public void Sign_SignatureLocation()
    {
        var xml = "<xml ID=\"id123\"><a/><b/></xml>";

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        var aNode = xmlDoc.DocumentElement!["a"]!;

        xmlDoc.DocumentElement!.Sign(TestData.Certificate, aNode);

        aNode.NextSibling!.LocalName.Should().Be("Signature");
    }

    [Fact]
    public void Sign_SignatureLocation_Null()
    {
        var xml = "<xml ID=\"id123\"/>";

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        xmlDoc.DocumentElement!.Invoking(de => de.Sign(TestData.Certificate, null!))
            .Should().Throw<ArgumentNullException>().WithParameterName("insertAfter");
    }

    [Fact]
    public void Sign_SignatureLocation_ImmediateChild()
    {
        var xml = "<xml ID=\"id123\"><a><b/></a></xml>";

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        var bNode = xmlDoc.DocumentElement!["a"]!["b"]!;

        xmlDoc.DocumentElement!.Invoking(de => de.Sign(TestData.Certificate, bNode))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void VerifySignature()
    {
        var element = SignedXmlHelperTests.GetSignatureElement();

        var (error, workingKey) =
            element.VerifySignature(TestData.SingleSigningKey, allowedHashes);

        error.Should().BeNull();
        workingKey.Should().BeSameAs(TestData.SigningKey);
    }

    [Fact]
    public void VerifySignature_WrongCert()
    {
        var element = SignedXmlHelperTests.GetSignatureElement();

        var (error, workingKey) = element.VerifySignature(TestData.SingleSigningKey2, allowedHashes);

        error.Should().Contain("key");
        workingKey.Should().BeNull();
    }

    [Fact]
    public void VerifySignature_TamperedData()
    {
        var element = SignedXmlHelperTests.GetSignatureElement();

        ((XmlElement)element.ParentNode!).SetAttribute("foo", "bar");

        var (error, workingKey) = element.VerifySignature(TestData.SingleSigningKey, allowedHashes);

        error.Should().Contain("didn't verify");
        workingKey.Should().BeNull();
    }

    [Fact]
    public void VerifySignature_SignatureWrapping()
    {
        var xml = "<xml ID=\"someID\"><content ID=\"content1\">text</content>"
            + "<injected>other text</injected></xml>";

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        var content = xmlDoc.DocumentElement!["content"]!;
        content.Sign(TestData.Certificate);

        // An XML wrapping attack is created by taking a legitimate signature
        // and putting it in another element. If the reference of the signature
        // is not properly checked, the element containing the signature
        // is incorrectly trusted.
        var signatureNode = content["Signature", SignedXml.XmlDsigNamespaceUrl]!;
        content.RemoveChild(signatureNode);

        var injected = xmlDoc.DocumentElement!["injected"]!;
        injected.AppendChild(signatureNode);

        var (error, workingKey) = injected["Signature", SignedXml.XmlDsigNamespaceUrl]!
            .VerifySignature(TestData.SingleSigningKey, allowedHashes);

        error.Should().Contain("reference");
        workingKey.Should().BeSameAs(TestData.SigningKey);
    }

    [Fact]
    public void VerifySignature_NoReference()
    {
        var signedWithoutReference = File.ReadAllText("../../../Xml/SignedXmlHelperTests/VerifySignature_NoReference.xml");

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(signedWithoutReference);

        var signature = xmlDoc.DocumentElement!["Signature", SignedXml.XmlDsigNamespaceUrl]!;

        signature.VerifySignature(TestData.SingleSigningKey, allowedHashes)
            .Error.Should().Contain("reference");
    }

    [Fact]
    public void VerifySignature_EmptyReferenceUri()
    {
        // While an empty reference Uri is allowed in the SignedXml general spec
        // and means the root of the containing document, it is explicitly not
        // allowed by the Saml2 Xml Signature processing rules.
        var xml = "<xml/>";

        var xd = new XmlDocument();
        xd.LoadXml(xml);

        var signedXml = new SignedXml(xd)
        {
            SigningKey = TestData.Certificate.GetRSAPrivateKey()
        };

        var reference = new Reference("");
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigExcC14NTransform());
        signedXml.AddReference(reference);

        signedXml.ComputeSignature();

        xd.DocumentElement!.AppendChild(signedXml.GetXml());

        xd.DocumentElement["Signature"]!.VerifySignature(
            TestData.SingleSigningKey, allowedHashes)
            .Error.Should().Match("Empty reference*");
    }

    [Fact]
    public void VerifySignature_MultipleReferences()
    {
        // While the SignedXml general spec allows multiple references, the
        // Saml2 Xml Signature processing rules do not.

        var xml = "<xml><a ID=\"a\"/><b ID=\"b\"/></xml>";

        var xd = new XmlDocument();
        xd.LoadXml(xml);

        var signedXml = new SignedXml(xd)
        {
            SigningKey = TestData.Certificate.GetRSAPrivateKey()
        };

        var reference = new Reference("#a");
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigExcC14NTransform());
        signedXml.AddReference(reference);

        reference = new Reference("#b");
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigExcC14NTransform());
        signedXml.AddReference(reference);

        signedXml.ComputeSignature();

        xd.DocumentElement!.AppendChild(signedXml.GetXml());

        xd.DocumentElement["Signature"]!.VerifySignature(
            TestData.SingleSigningKey, allowedHashes)
            .Error.Should().Match("*Signature*one reference*");
    }

    [Fact]
    public void VerifySignature_IncorrectTransforms()
    {
        // SAML2 Core 5.4.4 states that signatures SHOULD NOT contain other transforms than
        // the enveloped signature or exclusive canonicalization transforms and that a verifier
        // of a signature MAY reject signatures with other transforms. We'll reject them to
        // mitigate the risk of transforms opening up for assertion injections.

        var xml = "<xml ID=\"x\"/>";

        var xd = new XmlDocument();
        xd.LoadXml(xml);

        var signedXml = new SignedXml(xd)
        {
            SigningKey = TestData.Certificate.GetRSAPrivateKey()
        };

        var reference = new Reference("#x");
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigC14NTransform());
        signedXml.AddReference(reference);

        signedXml.ComputeSignature();

        xd.DocumentElement!.AppendChild(signedXml.GetXml());

        xd.DocumentElement["Signature"]!.VerifySignature(
            TestData.SingleSigningKey, allowedHashes)
            .Error.Should().Match("Transform*");
    }

    [Fact]
    public void VerifySignature_MultipleSignaturesInSameDocument()
    {
        var xml = "<xml><a ID=\"a\"/><b ID=\"b\"/></xml>";

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        var elemA = xmlDoc.DocumentElement!["a"]!;
        var elemB = xmlDoc.DocumentElement!["b"]!;

        elemA.Sign(TestData.Certificate);
        elemB.Sign(TestData.Certificate);

        elemA["Signature"]!.VerifySignature(TestData.SingleSigningKey, allowedHashes)
            .Error.Should().BeNull("First signature should validate");
        elemB["Signature"]!.VerifySignature(TestData.SingleSigningKey, allowedHashes)
            .Error.Should().BeNull("Second signature should validate");
    }

    [Fact]
    public void VerifySignature_NestedSignaturesInSameDocument()
    {
        var xml = "<xml><a ID=\"a\"><b ID=\"b\"/></a></xml>";

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        var elemA = xmlDoc.DocumentElement!["a"]!;
        var elemB = xmlDoc.DocumentElement!["a"]!["b"]!;

        // Important - sign nested element first.
        elemB.Sign(TestData.Certificate);
        elemA.Sign(TestData.Certificate);

        elemA["Signature"]!.VerifySignature(TestData.SingleSigningKey, allowedHashes)
            .Error.Should().BeNull("First signature should validate");
        elemB["Signature"]!.VerifySignature(TestData.SingleSigningKey, allowedHashes)
            .Error.Should().BeNull("Second signature should validate");
    }

    [Fact]
    public void VerifySignature_SigningAlgorithmStrength()
    {
        var xml = "<xml ID=\"x\"/>";

        var xd = new XmlDocument();
        xd.LoadXml(xml);

        var signedXml = new SignedXml(xd)
        {
            SigningKey = TestData.Certificate.GetRSAPrivateKey()
        };

        var reference = new Reference("#x");
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigExcC14NTransform());
        signedXml.AddReference(reference);

        signedXml.SignedInfo!.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;
        signedXml.ComputeSignature();

        xd.DocumentElement!.AppendChild(signedXml.GetXml());

        xd.DocumentElement["Signature"]!
            .VerifySignature(TestData.SingleSigningKey, allowedHashes)
            .Error.Should().Match("Signature*algorithm*sha1*");
    }

    [Fact]
    public void VerifySignature_DigestAlgorithmStrength()
    {
        var xml = "<xml ID=\"x\"/>";

        var xd = new XmlDocument();
        xd.LoadXml(xml);

        var signedXml = new SignedXml(xd)
        {
            SigningKey = TestData.Certificate.GetRSAPrivateKey()
        };

        var reference = new Reference("#x");
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigExcC14NTransform());
        reference.DigestMethod = SignedXml.XmlDsigSHA1Url;
        signedXml.AddReference(reference);

        signedXml.ComputeSignature();

        xd.DocumentElement!.AppendChild(signedXml.GetXml());

        xd.DocumentElement["Signature"]!
            .VerifySignature(TestData.SingleSigningKey, allowedHashes)
            .Error.Should().Match("Digest*algorithm*sha1*");
    }

    [Fact]
    public void IsSignedByAny()
    {
        var xd = CreateSignedDocument();

        var keys = new[]
        {
            TestData.SigningKey2,
            TestData.SigningKey
        };

        xd.DocumentElement!["Signature"]!.VerifySignature(keys, allowedHashes)
            .Error.Should().BeNull();
    }

    [Fact]
    public void IsSignedByAny_NoKeyMatches_ButEmbeddedKeyValidatesContent()
    {
        var xml = "<xml ID=\"a\"/>";

        var xd = new XmlDocument();
        xd.LoadXml(xml);

        xd.DocumentElement!.Sign(TestData.Certificate);

        xd.DocumentElement!["Signature"]!.VerifySignature(TestData.SingleSigningKey2, allowedHashes)
            .Error.Should().Match("*validated*contained*trusted*");
    }

    [Fact]
    public void VerifySignature_RejectsDuplicateIdsEvenIfCaseDiffer()
    {
        // Ensure that a document where referenced id exists in multiple
        // locations but ID is spelled Id or id is rejected. Allowing this is a
        // questionable design of SignedXml that Microsoft is not fixing.

        var xml = "<xml><b ID=\"a\"/></xml>";

        var xd = new XmlDocument();
        xd.LoadXml(xml);

        var signedXml = new SignedXml(xd)
        {
            SigningKey = TestData.Certificate.GetRSAPrivateKey()
        };

        var reference = new Reference("#a");
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigExcC14NTransform());
        signedXml.AddReference(reference);

        signedXml.ComputeSignature();

        var b = xd.DocumentElement!["b"]!;

        // Need to add the extra node after the signature was created.
        var a = xd.CreateElement("a");
        a.SetAttribute("Id", "a");
        xd.DocumentElement.AppendChild(a);

        b.AppendChild(signedXml.GetXml());

        b["Signature"]!.Invoking(s => s.VerifySignature(TestData.SingleSigningKey, allowedHashes))
            .Should().Throw<CryptographicException>()
            .WithMessage("Reference*one*");
    }

    [Fact]
    public void VerifySignature_RejectXmlLowerCaseIDs()
    {
        var xml = "<xml id=\"a\"/>";

        var xd = new XmlDocument();
        xd.LoadXml(xml);

        var signedXml = new SignedXml(xd)
        {
            SigningKey = TestData.Certificate.GetRSAPrivateKey()
        };

        var reference = new Reference("#a");
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigExcC14NTransform());
        signedXml.AddReference(reference);

        signedXml.ComputeSignature();

        xd.DocumentElement!.AppendChild(signedXml.GetXml());

        xd.DocumentElement!["Signature"]!
            .Invoking(s => s.VerifySignature(TestData.SingleSigningKey, allowedHashes))
            .Should().Throw<CryptographicException>()
            .WithMessage("Reference*ID*uppercase*");
    }

    private class SignRootSignedXml : SignedXml
    {
        public SignRootSignedXml(XmlDocument xd) : base(xd)
        { }

        // This allows us to create a signature with invalid reference for the
        // VerifySignature_RequiresReferenceNCName test.
        public override XmlElement GetIdElement(XmlDocument? document, string idValue)
            => document!.DocumentElement!;
    }

    [Fact]
    public void VerifySignature_RequiresReferenceNCName()
    {
        // Verify an Non-NCName for the ID doesn't pass.
        var xml = "<xml ID=\"urn:a\"/>";

        var xd = new XmlDocument();
        xd.LoadXml(xml);

        var signedXml = new SignRootSignedXml(xd)
        {
            SigningKey = TestData.Certificate.GetRSAPrivateKey()
        };

        var reference = new Reference("#urn:a");

        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigExcC14NTransform());
        signedXml.AddReference(reference);

        signedXml.ComputeSignature();

        xd.DocumentElement!.AppendChild(signedXml.GetXml());

        xd.DocumentElement!["Signature"]!
            .Invoking(s => s.VerifySignature(TestData.SingleSigningKey, allowedHashes))
            .Should().Throw<XmlException>();
    }
}