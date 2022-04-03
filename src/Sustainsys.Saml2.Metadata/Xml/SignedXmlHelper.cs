using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sustainsys.Saml2.Metadata.Xml;

/// <summary>
/// Helpers for SignedXml
/// </summary>
public static class SignedXmlHelper
{
    /// <summary>
    /// Adds an envoleped signature to the node.
    /// </summary>
    /// <param name="element">Element to sign</param>
    /// <param name="certificate">Certificate to use to sign</param>
    public static void Sign(this XmlElement element, X509Certificate2 certificate)
    {
        var signedXml = new SignedXml(element.OwnerDocument);

        switch (certificate.PublicKey.Oid.FriendlyName)
        {
            case "RSA":
                signedXml.SigningKey = certificate.GetRSAPrivateKey();
                break;
            default:
                throw new NotImplementedException();
        }

        var id = element.Attributes!["ID"]?.Value;

        var reference = new Reference($"#{id}");
        signedXml.AddReference(reference);
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigExcC14NTransform());

        signedXml.ComputeSignature();

        element.AppendChild(signedXml.GetXml());
    }

    /// <summary>
    /// Verifies a found Xml signature.
    /// </summary>
    /// <param name="signatureElement">The signature element to verify.</param>
    /// <param name="key">They key to use to verify.</param>
    /// <returns>Typle with bool flag if the signature is valid, possibly error message,
    /// TrustLevel of the signed data (based on the key's trust level) and the element
    /// that is signed.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static (string? Error, bool KeyWorked, TrustLevel, XmlElement? SignedElement) 
        VerifySignature(this XmlElement signatureElement, SigningKey key)
    {
        var signedXml = new SignedXml(signatureElement.OwnerDocument);

        signedXml.LoadXml(signatureElement);

        // All versions of .NET prior to .NET 7 contains a bug that only lets the first signature in a
        // document be validated, we want a workaround for that. For .NET 7+ I contributed
        // this fix to the System.Security.Cryptography.Xml library.
#if !NET7_0_OR_GREATER
        FixSignatureIndex(signedXml, signatureElement);
#endif

        string? error = null;
        bool keyWorked = signedXml.CheckSignature(key.Key);
        XmlElement? signedElement = null;

        if (!keyWorked)
        {
            error = "Signature didn't verify for the specified key. ";
        }

        if(signedXml.SignedInfo.References.Count != 1)
        {
            error += "The Signature should contain exactly one reference. ";
        }
        else
        {
            var reference = (Reference)signedXml.SignedInfo.References[0]!;
            var id = reference.Uri.Substring(1); // Drop of the #

            signedElement = signedXml.GetIdElement(signatureElement.OwnerDocument, id);

            if(signedElement == null || signedElement != signatureElement.ParentNode)
            {
                error += "Incorrect reference on Xml Signature, the reference must be to the parent element of the signature. ";
            }
        }

        var valid = string.IsNullOrEmpty(error);

        return (
            error?.TrimEnd(),
            keyWorked, 
            valid ? key.TrustLevel : TrustLevel.None,
            valid ? signedElement : null);
    }

    static readonly PropertyInfo? signaturePosition = typeof(XmlDsigEnvelopedSignatureTransform)
    .GetProperty("SignaturePosition", BindingFlags.Instance | BindingFlags.NonPublic);

    /// <summary>
    /// Workaround for a bug in Reference.LoadXml incorrectly counting index
    /// of signature from the start of the document, not from the start of
    /// the element.
    /// </summary>
    /// <param name="signedXml">SignedXml</param>
    /// <param name="signatureElement">Signature element.</param>
    private static void FixSignatureIndex(SignedXml signedXml, XmlElement signatureElement)
    {
        if (signaturePosition == null)
            return;

        var nsm = new XmlNamespaceManager(signatureElement.OwnerDocument.NameTable);
        nsm.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);

        var signaturesInParent = signatureElement.ParentNode!.SelectNodes(".//ds:Signature", nsm)!;

        int correctSignaturePosition = 0;
        for (int i = 0; i < signaturesInParent.Count; i++)
        {
            if (signaturesInParent[i] == signatureElement)
            {
                correctSignaturePosition = i + 1;
            }
        }

        foreach (var t in ((Reference)signedXml.SignedInfo.References[0]!).TransformChain)
        {
            if(t is XmlDsigEnvelopedSignatureTransform envelopedTransform)
            {
                signaturePosition.SetValue(envelopedTransform, correctSignaturePosition);
            }
        }
    }
}
