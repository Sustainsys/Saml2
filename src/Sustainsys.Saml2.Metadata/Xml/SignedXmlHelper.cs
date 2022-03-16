using System;
using System.Collections.Generic;
using System.Linq;
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
                error += "Incorrect reference on Xml Signature. The reference must be to the parent element of the signature. ";
            }
        }

        var valid = string.IsNullOrEmpty(error);

        return (
            error,
            keyWorked, 
            valid ? key.TrustLevel : TrustLevel.None,
            valid ? signedElement : null);
    }
}
