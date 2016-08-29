using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using FluentAssertions;
using Kentor.AuthServices.Tests.Helpers;
using Kentor.AuthServices.Exceptions;
using System.Security.Cryptography;
using System.Reflection;
using Kentor.AuthServices.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class XmlHelpersTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            SignedXmlHelper.RemoveGlobalSha256XmlSignatureSupport();
        }

        public static readonly X509Certificate2 TestCert = new X509Certificate2("Kentor.AuthServices.Tests.pfx");

        [TestMethod]
        public void XmlHelpers_Sign_Nullcheck_xmlDocument()
        {
            XmlDocument xd = null;
            Action a = () => xd.Sign(TestCert);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("xmlDocument");
        }

        [TestMethod]
        public void XmlHelpers_Sign_Nullcheck_xmlElement()
        {
            ((XmlElement)null).Invoking(
                x => x.Sign(SignedXmlHelper.TestCert, true))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("xmlElement");
        }

        [TestMethod]
        public void XmlHelpers_Sign_Nullcheck_Cert()
        {
            xmlDocument.DocumentElement.Invoking(
                x => x.Sign(null, false))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("cert");
        }

        [TestMethod]
        public void XmlHelpers_Sign()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<root ID=\"rootElementId\"><content>Some Content</content></root>");

            xmlDoc.Sign(TestCert);

            var signature = xmlDoc.DocumentElement["Signature", SignedXml.XmlDsigNamespaceUrl];

            signature["SignedInfo", SignedXml.XmlDsigNamespaceUrl]
                ["Reference", SignedXml.XmlDsigNamespaceUrl].Attributes["URI"].Value
                .Should().Be("#rootElementId");

            var signedXml = new SignedXml(xmlDoc);
            signedXml.LoadXml(signature);
            signedXml.CheckSignature(TestCert, true).Should().BeTrue();
        }

        const string xmlString = "<xml a=\"b\">\n  <indented>content</indented>\n</xml>";
        readonly XmlDocument xmlDocument = XmlHelpers.FromString(xmlString);

        [TestMethod]
        public void XmlHelpers_FromString()
        {
            xmlDocument.OuterXml.Should().Be(xmlString);
        }

        [TestMethod]
        public void XmlHelpers_Remove_NullcheckAttribute()
        {
            ((XmlAttributeCollection)null).Invoking(
                a => a.Remove("attributeName"))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("attributes");
        }

        [TestMethod]
        public void XmlHelpers_Remove_NullcheckAttributeName()
        {
            xmlDocument.DocumentElement.Attributes.Invoking(
                a => a.Remove(attributeName: null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("attributeName");
        }

        [TestMethod]
        public void XmlHelpers_RemoveChild_NullcheckXmlElement()
        {
            new XmlDocument().DocumentElement.Invoking(
                e => e.RemoveChild("name", "ns"))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("xmlElement");
        }

        [TestMethod]
        public void XmlHelpers_RemoveChild_NullcheckName()
        {
            xmlDocument.DocumentElement.Invoking(
                e => e.RemoveChild(null, "ns"))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("name");
        }

        [TestMethod]
        public void XmlHelpers_RemoveChild_NullcheckNs()
        {
            xmlDocument.DocumentElement.Invoking(
                e => e.RemoveChild("name", null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("ns");
        }

        [TestMethod]
        public void XmlHelpers_IsSignedBy_NullcheckXmlElement()
        {
            ((XmlElement)null).Invoking(
                x => x.IsSignedBy(SignedXmlHelper.TestCert))
                .ShouldThrow<ArgumentNullException>("xmlElement");
        }

        [TestMethod]
        public void XmlHelpers_IsSignedBy_NullcheckCertificate()
        {
            xmlDocument.DocumentElement.Invoking(
                x => x.IsSignedBy(null))
                .ShouldThrow<ArgumentNullException>("certificate");
        }

        [TestMethod]
        public void XmlHelpers_IsSignedBy()
        {
            var xml = "<xml ID=\"someID\"><content>text</content></xml>";
            var xmlDoc = XmlHelpers.FromString(xml);
            xmlDoc.Sign(SignedXmlHelper.TestCert);

            xmlDoc.DocumentElement.IsSignedBy(SignedXmlHelper.TestCert).Should().BeTrue();
        }

        [TestMethod]
        public void XmlHelpers_IsSignedBy_ThrowsOnWrongCert()
        {
            var xml = "<xml ID=\"someID\"><content>text</content></xml>";
            var xmlDoc = XmlHelpers.FromString(xml);
            xmlDoc.Sign(SignedXmlHelper.TestCert2, true);

            xmlDoc.DocumentElement.Invoking(
                x => x.IsSignedBy(SignedXmlHelper.TestCert))
                .ShouldThrow<InvalidSignatureException>()
                .And.Message.Should().Be("The signature verified correctly with the key contained in the signature, but that key is not trusted.");
        }

        [TestMethod]
        public void XmlHelpers_IsSignedBy_ThrowsOnTamperedData()
        {
            var xml = "<xml ID=\"someID\"><content>text</content></xml>";
            var xmlDoc = XmlHelpers.FromString(xml);
            xmlDoc.Sign(SignedXmlHelper.TestCert);

            xmlDoc.DocumentElement["content"].InnerText = "changedText";

            xmlDoc.DocumentElement.Invoking(
                x => x.IsSignedBy(SignedXmlHelper.TestCert))
                .ShouldThrow<InvalidSignatureException>()
                .And.Message.Should().Be("Signature didn't verify. Have the contents been tampered with?");
        }

        [TestMethod]
        public void XmlHelpers_IsSignedBy_TrowsOnSignatureWrapping()
        {
            var xml = "<xml ID=\"someID\"><content ID=\"content1\">text</content>"
                + "<injected>other text</injected></xml>";
            var xmlDoc = XmlHelpers.FromString(xml);

            xmlDoc.DocumentElement["content"].Sign(SignedXmlHelper.TestCert, false);

            // An XML wrapping attack is created by taking a legitimate signature
            // and putting it in another element. If the reference of the signature
            // is not properly checked, the element containing the signature
            // is incorrectly trusted.
            var signatureNode = xmlDoc.DocumentElement["content"]["Signature", SignedXml.XmlDsigNamespaceUrl];
            xmlDoc.DocumentElement["content"].RemoveChild(signatureNode);
            xmlDoc.DocumentElement["injected"].AppendChild(signatureNode);

            xmlDoc.DocumentElement["injected"].Invoking(
                x => x.IsSignedBy(SignedXmlHelper.TestCert))
                .ShouldThrow<InvalidSignatureException>()
                .And.Message.Should().Be("Incorrect reference on Xml signature. The reference must be to the root element of the element containing the signature.");
        }

        [TestMethod]
        public void XmlHelpers_IsSignedBy_FalseOnMissingSignature()
        {
            var xml = "<xml ID=\"someID\"><content>text</content></xml>";
            var xmlDoc = XmlHelpers.FromString(xml);

            xmlDoc.DocumentElement.IsSignedBy(SignedXmlHelper.TestCert).Should().BeFalse();
        }

        [TestMethod]
        public void XmlHelpers_IsSignedBy_ThrowsOnMissingReferenceInSignature()
        {
            var signedWithoutReference = @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol"" xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion"" ID=""Saml2Response_Validate_FalseOnMissingReference"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""><saml2:Issuer>https://idp.example.com</saml2:Issuer><saml2p:Status><saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" /></saml2p:Status><Signature xmlns=""http://www.w3.org/2000/09/xmldsig#""><SignedInfo><CanonicalizationMethod Algorithm=""http://www.w3.org/TR/2001/REC-xml-c14n-20010315"" /><SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1"" /></SignedInfo><SignatureValue>tYFIoYmrzmp3H7TXm9IS8DW3buBZIb6sI2ycrn+AOnVcdYnPTJpk3ntHlqQKXNEyXgXZNdqEuFpgI1I0P0TlhM+C3rBJnflkApkxZkak5RwnJzDWTHpsSDjYcm+/XgBy3JVZJuMWb2YPaV8GB6cjBMDrENUEaoKRg+FpzPUZO1EOMcqbocXp5cHie1CkPnD1OtT/cuzMBUMpBGZMxjZwdFpOO7R3CUXh/McxKfoGUQGC3DVpt5T8uGkpj4KqZVPS/qTCRhbPRDjg73BdWbdkFpFWge8G/FgkYxr9LBE1TsrxptppO9xoA5jXwJVZaWndSMvo6TuOjUgqY2w5RTkqhA==</SignatureValue></Signature></saml2p:Response>";

            var xmlDoc = XmlHelpers.FromString(signedWithoutReference);

            xmlDoc.DocumentElement.Invoking(
                x => x.IsSignedBy(SignedXmlHelper.TestCert))
                .ShouldThrow<InvalidSignatureException>()
                .And.Message.Should().Be("No reference found in Xml signature, it doesn't validate the Xml data.");
        }

        [TestMethod]
        public void XmlHelpers_IsSignedBy_ThrowsOnDualReferencesInSignature()
        {
            var xml = "<xml ID=\"myxml\" />";

            var xmlDoc = XmlHelpers.FromString(xml);

            var signedXml = new SignedXml(xmlDoc);
            signedXml.SigningKey = (RSACryptoServiceProvider)SignedXmlHelper.TestCert.PrivateKey;
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            var ref1 = new Reference { Uri = "#myxml" };
            ref1.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            ref1.AddTransform(new XmlDsigExcC14NTransform());
            signedXml.AddReference(ref1);

            var ref2 = new Reference { Uri = "#myxml" };
            ref2.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            ref2.AddTransform(new XmlDsigExcC14NTransform());
            signedXml.AddReference(ref2);

            signedXml.ComputeSignature();
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(signedXml.GetXml(), true));

            xmlDoc.DocumentElement.Invoking(
                x => x.IsSignedBy(SignedXmlHelper.TestCert))
                .ShouldThrow<InvalidSignatureException>()
                .And.Message.Should().Be("Multiple references for Xml signatures are not allowed.");
        }

        [TestMethod]
        public void XmlHelpers_IsSignedBy_ThrowsInformativeMessageOnSha256Signature()
        {
            // With .Net 4.6.2 and above this test will not throw any error because the SHA256 is now built-in
            if ( CryptoConfig.CreateFromName( "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256" ) != null ) return;

            var xmlSignedWithSha256 = @"<Assertion ID=""Saml2Response_GetClaims_ThrowsInformativeExceptionForSha256"" IssueInstant=""2015-03-13T20:43:07.330Z"" Version=""2.0"" xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""><Issuer>https://idp.example.com</Issuer><Signature xmlns=""http://www.w3.org/2000/09/xmldsig#""><SignedInfo><CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /><SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"" /><Reference URI=""#Saml2Response_GetClaims_ThrowsInformativeExceptionForSha256""><Transforms><Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature"" /><Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /></Transforms><DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" /><DigestValue>F+E7u3vqMC07ipvP9AowsMqP7y6CsAC0GeEIxNSwDEI=</DigestValue></Reference></SignedInfo><SignatureValue>GmiXn24Ccnr64TbmDd1/nLM+891z0FtRHSpU8+75uOqbpNK/ZZGrltFf2YZ5u9b9O0HfbFFsZ0i28ocwAZOv2UfxQrCtOGf3ss7Q+t2Zmc6Q/3ES7HIa15I5BbaSdNfpOMlX6N1XXhMprRGy2YWMr5IAIhysFG1A2oHaC3yFiesfUrawN/lXUYuI22Kf4A5bmnIkKijnwX9ewnhRj6569bw+c6q+tVZSHQzI+KMU9KbKN4NsXxAmv6dM1w2qOiX9/CO9LzwEtlhA9yo3sl0uWP8z5GwK9qgOlsF2NdImAQ5f0U4Uv26doFn09W+VExFwNhcXhewQUuPBYBr+XXzdww==</SignatureValue><KeyInfo><X509Data><X509Certificate>MIIDIzCCAg+gAwIBAgIQg7mOjTf994NAVxZu4jqXpzAJBgUrDgMCHQUAMCQxIjAgBgNVBAMTGUtlbnRvci5BdXRoU2VydmljZXMuVGVzdHMwHhcNMTMwOTI1MTMzNTQ0WhcNMzkxMjMxMjM1OTU5WjAkMSIwIAYDVQQDExlLZW50b3IuQXV0aFNlcnZpY2VzLlRlc3RzMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwVGpfvK9N//MnA5Jo1q2liyPR24406Dp25gv7LB3HK4DWgqsb7xXM6KIV/WVOyCV2g/O1ErBlB+HLhVZ4XUJvbqBbgAJqFO+TZwcCIe8u4nTEXeU660FdtkKClA17sbtMrAGdDfOPwVBHSuavdHeD7jHNI4RUDGKnEW13/0EvnHDilIetwODRxrX/+41R24sJThFbMczByS3OAL2dcIxoAynaGeM90gXsVYow1QhJUy21+cictikb7jW4mW6dvFCBrWIceom9J295DcQIHoxJy5NoZwMir/JV00qs1wDVoN20Ve1DC5ImwcG46XPF7efQ44yLh2j5Yexw+xloA81dwIDAQABo1kwVzBVBgNVHQEETjBMgBAWIahoZhXVUogbAqkS7zwfoSYwJDEiMCAGA1UEAxMZS2VudG9yLkF1dGhTZXJ2aWNlcy5UZXN0c4IQg7mOjTf994NAVxZu4jqXpzAJBgUrDgMCHQUAA4IBAQA2aGzmuKw4AYXWMhrGj5+i8vyAoifUn1QVOFsUukEA77CrqhqqaWFoeagfJp/45vlvrfrEwtF0QcWfmO9w1VvHwm7sk1G/cdYyJ71sU+llDsdPZm7LxQvWZYkK+xELcinQpSwt4ExavS+jLcHoOYHYwIZMBn3U8wZw7Kq29oGnoFQz7HLCEl/G9i3QRyvFITNlWTjoScaqMjHTzq6HCMaRsL09DLcY3KB+cedfpC0/MBlzaxZv0DctTulyaDfM9DCYOyokGN/rQ6qkAR0DDm8fVwknbJY7kURXNGoUetulTb5ow8BvD1gncOaYHSD0kbHZG+bLsUZDFatEr2KW8jbG</X509Certificate></X509Data></KeyInfo></Signature><Subject><NameID>SomeUser</NameID><SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" /></Subject><Conditions NotOnOrAfter=""2100-01-01T05:00:00.000Z"" /></Assertion>";

            var xmlDoc = XmlHelpers.FromString(xmlSignedWithSha256);

            xmlDoc.DocumentElement.Invoking(
                x => x.IsSignedBy(SignedXmlHelper.TestCertSignOnly))
                .ShouldThrow<InvalidSignatureException>()
                .And.Message.Should().Be("SHA256 signatures require the algorithm to be registered at the process level. Upgrade to .Net 4.6.2 or call Kentor.AuthServices.Configuration.Options.GlobalEnableSha256XmlSignatures() on startup to register.");
        }

        [TestMethod]
        public void XmlHelpers_IsSignedBy_ThrowsOnTamperedData_WithSha256Signature()
        {
            Options.GlobalEnableSha256XmlSignatures();

            var xml = @"<Assertion ID=""Saml2Response_GetClaims_FailsSha256WhenChanged"" IssueInstant=""2015-03-13T20:44:00.791Z"" Version=""2.0"" xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""><Issuer>https://idp.example.com</Issuer><Signature xmlns=""http://www.w3.org/2000/09/xmldsig#""><SignedInfo><CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /><SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"" /><Reference URI=""#Saml2Response_GetClaims_FailsSha256WhenChanged""><Transforms><Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature"" /><Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /></Transforms><DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" /><DigestValue>BKRyWqweAczLA8fgRcx6zzMDiP0qT0TwqU/X4VgLiXM=</DigestValue></Reference></SignedInfo><SignatureValue>iK8s+MkLlixSSQu5Q/SHRZLhfnj4jlyPLAD6C2n9zmQu4CosZME7mxiNFiWyOE8XRGd+2LJle+NjJrkZFktVb03JaToq7w4Q8GfJ2oUUjNCweoaJ6NzsnwkFoXhyh0dfOixl/Ifa3qDX50/Hv2twF/QXfDs08GZTxZKehKsVDITyVd6nytF8VUb0+nU7UMWPn1XeHM7YNI/1mkVbCRx/ci5ZRxwjAX40xttd4JL6oBnp5oaaMgWpAa2cVb+t/9HhCRThEho1etbPHx/+E9ElL1PhKqKX6nh2GSH1TFJkwEXIPPZKqCs3YDINLBZpLfl626zbV4cGOGyWUAroVsk2uw==</SignatureValue><KeyInfo><X509Data><X509Certificate>MIIDIzCCAg+gAwIBAgIQg7mOjTf994NAVxZu4jqXpzAJBgUrDgMCHQUAMCQxIjAgBgNVBAMTGUtlbnRvci5BdXRoU2VydmljZXMuVGVzdHMwHhcNMTMwOTI1MTMzNTQ0WhcNMzkxMjMxMjM1OTU5WjAkMSIwIAYDVQQDExlLZW50b3IuQXV0aFNlcnZpY2VzLlRlc3RzMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwVGpfvK9N//MnA5Jo1q2liyPR24406Dp25gv7LB3HK4DWgqsb7xXM6KIV/WVOyCV2g/O1ErBlB+HLhVZ4XUJvbqBbgAJqFO+TZwcCIe8u4nTEXeU660FdtkKClA17sbtMrAGdDfOPwVBHSuavdHeD7jHNI4RUDGKnEW13/0EvnHDilIetwODRxrX/+41R24sJThFbMczByS3OAL2dcIxoAynaGeM90gXsVYow1QhJUy21+cictikb7jW4mW6dvFCBrWIceom9J295DcQIHoxJy5NoZwMir/JV00qs1wDVoN20Ve1DC5ImwcG46XPF7efQ44yLh2j5Yexw+xloA81dwIDAQABo1kwVzBVBgNVHQEETjBMgBAWIahoZhXVUogbAqkS7zwfoSYwJDEiMCAGA1UEAxMZS2VudG9yLkF1dGhTZXJ2aWNlcy5UZXN0c4IQg7mOjTf994NAVxZu4jqXpzAJBgUrDgMCHQUAA4IBAQA2aGzmuKw4AYXWMhrGj5+i8vyAoifUn1QVOFsUukEA77CrqhqqaWFoeagfJp/45vlvrfrEwtF0QcWfmO9w1VvHwm7sk1G/cdYyJ71sU+llDsdPZm7LxQvWZYkK+xELcinQpSwt4ExavS+jLcHoOYHYwIZMBn3U8wZw7Kq29oGnoFQz7HLCEl/G9i3QRyvFITNlWTjoScaqMjHTzq6HCMaRsL09DLcY3KB+cedfpC0/MBlzaxZv0DctTulyaDfM9DCYOyokGN/rQ6qkAR0DDm8fVwknbJY7kURXNGoUetulTb5ow8BvD1gncOaYHSD0kbHZG+bLsUZDFatEr2KW8jbG</X509Certificate></X509Data></KeyInfo></Signature><Subject><NameID>SomeUser</NameID><SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" /></Subject><Conditions NotOnOrAfter=""2100-01-01T05:00:00.000Z"" /></Assertion>";
            xml = xml.Replace("SomeUser", "OtherUser");

            var xmlDoc = XmlHelpers.FromString(xml);

            xmlDoc.DocumentElement.Invoking(
                x => x.IsSignedBy(SignedXmlHelper.TestCert))
                .ShouldThrow<InvalidSignatureException>()
                .WithMessage("Signature didn't verify. Have the contents been tampered with?");
        }

        [TestMethod]
        public void XmlHelpers_IsSignedBy_ThrowsOnIncorrectTransformsInSignature()
        {
            // SAML2 Core 5.4.4 states that signatures SHOULD NOT contain other transforms than
            // the enveloped signature or exclusive canonicalization transforms and that a verifier
            // of a signature MAY reject signatures with other transforms. We'll reject them to
            // mitigate the risk of transforms opening up for assertion injections.

            var xml = "<xml ID=\"MyXml\" />";

            var xmlDoc = XmlHelpers.FromString(xml);

            var signedXml = new SignedXml(xmlDoc);
            signedXml.SigningKey = (RSACryptoServiceProvider)SignedXmlHelper.TestCert.PrivateKey;
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            var reference = new Reference { Uri = "#MyXml" };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform()); // The allowed transform is XmlDsigExcC14NTransform
            signedXml.AddReference(reference);

            signedXml.ComputeSignature();
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(signedXml.GetXml(), true));

            xmlDoc.DocumentElement.Invoking(
                x => x.IsSignedBy(SignedXmlHelper.TestCert))
                .ShouldThrow<InvalidSignatureException>()
                .And.Message.Should().Be("Transform \"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" found in Xml signature SHOULD NOT be used with SAML2.");
        }

        [TestMethod]
        public void XmlHelpers_IsSignedBy_HandlesMultipleSignaturesInSameDocument()
        {
            var content1 = SignedXmlHelper.SignXml("<content ID=\"c1\" />");
            var content2 = SignedXmlHelper.SignXml("<content ID=\"c2\" />");

            var xml = 
$@"<xml>
    {content1}
    {content2}
</xml>";

            var xmlDoc = XmlHelpers.FromString(xml);

            ((XmlElement)xmlDoc.SelectSingleNode("//*[@ID=\"c1\"]")).IsSignedBy(SignedXmlHelper.TestCert)
                .Should().BeTrue("first content is correclty signed");

            ((XmlElement)xmlDoc.SelectSingleNode("//*[@ID=\"c2\"]")).IsSignedBy(SignedXmlHelper.TestCert)
                .Should().BeTrue("second content is correclty signed");
        }

        [TestMethod]
        public void XmlHelpers_IsSignedBy_DoesNotThrowSha256MessageForOtherProblem()
        {
            Options.GlobalEnableSha256XmlSignatures();

            // Here I've specified an invalid digest algorithm. Want to be sure it
            // does NOT flag this as a problem with the sha256 signature algorithm 
            // (they both throw CryptographicException)
            var xml =
                @"<Assertion ID=""Saml2Response_GetClaims_DoesNotThrowSha256MessageForOtherProblem_Assertion"" IssueInstant=""2015-03-13T20:43:07.330Z"" Version=""2.0"" xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""><Issuer>https://idp.example.com</Issuer><Signature xmlns=""http://www.w3.org/2000/09/xmldsig#""><SignedInfo><CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /><SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#SomeInvalidName"" /><Reference URI=""#Saml2Response_GetClaims_DoesNotThrowSha256MessageForOtherProblem_Assertion""><Transforms><Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature"" /><Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /></Transforms><DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" /><DigestValue>F+E7u3vqMC07ipvP9AowsMqP7y6CsAC0GeEIxNSwDEI=</DigestValue></Reference></SignedInfo><SignatureValue>GmiXn24Ccnr64TbmDd1/nLM+891z0FtRHSpU8+75uOqbpNK/ZZGrltFf2YZ5u9b9O0HfbFFsZ0i28ocwAZOv2UfxQrCtOGf3ss7Q+t2Zmc6Q/3ES7HIa15I5BbaSdNfpOMlX6N1XXhMprRGy2YWMr5IAIhysFG1A2oHaC3yFiesfUrawN/lXUYuI22Kf4A5bmnIkKijnwX9ewnhRj6569bw+c6q+tVZSHQzI+KMU9KbKN4NsXxAmv6dM1w2qOiX9/CO9LzwEtlhA9yo3sl0uWP8z5GwK9qgOlsF2NdImAQ5f0U4Uv26doFn09W+VExFwNhcXhewQUuPBYBr+XXzdww==</SignatureValue><KeyInfo><X509Data><X509Certificate>MIIDIzCCAg+gAwIBAgIQg7mOjTf994NAVxZu4jqXpzAJBgUrDgMCHQUAMCQxIjAgBgNVBAMTGUtlbnRvci5BdXRoU2VydmljZXMuVGVzdHMwHhcNMTMwOTI1MTMzNTQ0WhcNMzkxMjMxMjM1OTU5WjAkMSIwIAYDVQQDExlLZW50b3IuQXV0aFNlcnZpY2VzLlRlc3RzMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwVGpfvK9N//MnA5Jo1q2liyPR24406Dp25gv7LB3HK4DWgqsb7xXM6KIV/WVOyCV2g/O1ErBlB+HLhVZ4XUJvbqBbgAJqFO+TZwcCIe8u4nTEXeU660FdtkKClA17sbtMrAGdDfOPwVBHSuavdHeD7jHNI4RUDGKnEW13/0EvnHDilIetwODRxrX/+41R24sJThFbMczByS3OAL2dcIxoAynaGeM90gXsVYow1QhJUy21+cictikb7jW4mW6dvFCBrWIceom9J295DcQIHoxJy5NoZwMir/JV00qs1wDVoN20Ve1DC5ImwcG46XPF7efQ44yLh2j5Yexw+xloA81dwIDAQABo1kwVzBVBgNVHQEETjBMgBAWIahoZhXVUogbAqkS7zwfoSYwJDEiMCAGA1UEAxMZS2VudG9yLkF1dGhTZXJ2aWNlcy5UZXN0c4IQg7mOjTf994NAVxZu4jqXpzAJBgUrDgMCHQUAA4IBAQA2aGzmuKw4AYXWMhrGj5+i8vyAoifUn1QVOFsUukEA77CrqhqqaWFoeagfJp/45vlvrfrEwtF0QcWfmO9w1VvHwm7sk1G/cdYyJ71sU+llDsdPZm7LxQvWZYkK+xELcinQpSwt4ExavS+jLcHoOYHYwIZMBn3U8wZw7Kq29oGnoFQz7HLCEl/G9i3QRyvFITNlWTjoScaqMjHTzq6HCMaRsL09DLcY3KB+cedfpC0/MBlzaxZv0DctTulyaDfM9DCYOyokGN/rQ6qkAR0DDm8fVwknbJY7kURXNGoUetulTb5ow8BvD1gncOaYHSD0kbHZG+bLsUZDFatEr2KW8jbG</X509Certificate></X509Data></KeyInfo></Signature><Subject><NameID>SomeUser</NameID><SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" /></Subject><Conditions NotOnOrAfter=""2100-01-01T05:00:00.000Z"" /></Assertion>";

            var xmlDoc = XmlHelpers.FromString(xml);

            xmlDoc.DocumentElement.Invoking(x => x.IsSignedBy(SignedXmlHelper.TestCert))
                .ShouldThrow<CryptographicException>()
                .WithMessage("SignatureDescription could not be created for the signature algorithm supplied.");
        }

        [TestMethod]
        public void XmlHelpers_IsSignedByAny_ThrowsOnCertValidationWithRsaKey()
        {
            var xml = "<xml ID=\"MyXml\" />";

            var xmlDoc = XmlHelpers.FromString(xml);
            xmlDoc.Sign(SignedXmlHelper.TestCert, false);

            var signingKeys = Enumerable.Repeat(SignedXmlHelper.TestKey, 1);

            xmlDoc.DocumentElement.Invoking(x => x.IsSignedByAny(signingKeys, true))
                .ShouldThrow<InvalidOperationException>()
                .And.Message.Should().Be("Certificate validation enabled, but the signing key identifier is of type RsaKeyIdentifierClause which cannot be validated as a certificate.");
        }
    }
}