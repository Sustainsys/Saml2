using FluentAssertions;
using Sustainsys.Saml2.Metadata.Xml;
using Sustainsys.Saml2.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Sustainsys.Saml2.Metadata.Tests.Xml;

public class SignedXmlHelperTests
{
    private XmlDocument CreateSignedDocument()
    {
        var xml = "<xml ID=\"id123\"/>";

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        xmlDoc.DocumentElement!.Sign(TestData.Certificate);

        return xmlDoc;
    }

    private XmlElement GetSignatureElement()
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
    public void VerifySignature()
    {
        var element = GetSignatureElement();

        var (error, keyWorked, trustLevel, signedElement) =
            element.VerifySignature(TestData.SigningKey);

        error.Should().BeNull();
        keyWorked.Should().BeTrue();
        trustLevel.Should().Be(TestData.SigningKey.TrustLevel);
        signedElement.Should().BeSameAs((XmlElement)element.ParentNode!);
    }

    [Fact]
    public void VerifySignature_WrongCert()
    {
        var element = GetSignatureElement();

        var (error, keyWorked, _, _) = element.VerifySignature(TestData.SigningKey2);

        error.Should().Contain("key");
        keyWorked.Should().BeFalse();
    }

    [Fact]
    public void VerifySignature_TamperedData()
    {
        var element = GetSignatureElement();

        ((XmlElement)element.ParentNode!).SetAttribute("foo", "bar");

        var (error, keyWorked, _, _) = element.VerifySignature(TestData.SigningKey);

        error.Should().Contain("didn't verify");
        keyWorked.Should().BeFalse();
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

        var (error, keyWorked, _, _) = injected["Signature", SignedXml.XmlDsigNamespaceUrl]!
            .VerifySignature(TestData.SigningKey);

        error.Should().Contain("reference");
        keyWorked.Should().BeTrue();
    }

    [Fact]
    public void VerifySignature_NoReference()
    {
        var signedWithoutReference = @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol"" xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion"" ID=""Saml2Response_Validate_FalseOnMissingReference"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""><saml2:Issuer>https://idp.example.com</saml2:Issuer><saml2p:Status><saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" /></saml2p:Status><Signature xmlns=""http://www.w3.org/2000/09/xmldsig#""><SignedInfo><CanonicalizationMethod Algorithm=""http://www.w3.org/TR/2001/REC-xml-c14n-20010315"" /><SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1"" /></SignedInfo><SignatureValue>tYFIoYmrzmp3H7TXm9IS8DW3buBZIb6sI2ycrn+AOnVcdYnPTJpk3ntHlqQKXNEyXgXZNdqEuFpgI1I0P0TlhM+C3rBJnflkApkxZkak5RwnJzDWTHpsSDjYcm+/XgBy3JVZJuMWb2YPaV8GB6cjBMDrENUEaoKRg+FpzPUZO1EOMcqbocXp5cHie1CkPnD1OtT/cuzMBUMpBGZMxjZwdFpOO7R3CUXh/McxKfoGUQGC3DVpt5T8uGkpj4KqZVPS/qTCRhbPRDjg73BdWbdkFpFWge8G/FgkYxr9LBE1TsrxptppO9xoA5jXwJVZaWndSMvo6TuOjUgqY2w5RTkqhA==</SignatureValue></Signature></saml2p:Response>";

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(signedWithoutReference);

        var signature = xmlDoc.DocumentElement!["Signature", SignedXml.XmlDsigNamespaceUrl]!;

        var (error, _, _, _) = signature.VerifySignature(TestData.SigningKey);

        error.Should().Contain("reference");
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

        var (error, _, _, _) = xd.DocumentElement["Signature"]!.VerifySignature(TestData.SigningKey);

        error.Should().Match("Empty reference*");
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

        var (error, _, _, _) = xd.DocumentElement["Signature"]!.VerifySignature(TestData.SigningKey);

        error.Should().Match("*Signature*one reference*");
    }

    [Fact(Skip = "Test Not Implemented")]
    public void VerifySignature_IncorrectTransforms()
    {
        // SAML2 Core 5.4.4 states that signatures SHOULD NOT contain other transforms than
        // the enveloped signature or exclusive canonicalization transforms and that a verifier
        // of a signature MAY reject signatures with other transforms. We'll reject them to
        // mitigate the risk of transforms opening up for assertion injections.
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

        elemA["Signature"]!.VerifySignature(TestData.SigningKey).Error.Should().BeNull();
        elemB["Signature"]!.VerifySignature(TestData.SigningKey).Error.Should().BeNull();
    }

    [Fact(Skip = "Test Not Implemented")]
    public void VerifySignature_SigningAlgorithmStrength()
    { }

    [Fact(Skip = "Test Not Implemented")]
    public void VerifySignature_DigestAlgorithmStrength()
    { }

    [Fact(Skip = "Test Not Implemented")]
    public void IsSignedByAny()
    { }

    [Fact(Skip = "Test Not Implemented")]
    public void IsSignedByAny_NoKeyMatches()
    { }

    [Fact(Skip = "Test Not Implemented")]
    public void VerifySignaure_RejectsDuplicateIdsEvenIfCaseDiffer()
    {
        // Ensure that a document where referenced id exists in multiple
        // locations but ID is spelled Id or id is rejected.
    }
}