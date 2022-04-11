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
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigExcC14NTransform());
        signedXml.AddReference(reference);

        signedXml.KeyInfo.AddClause(new KeyInfoX509Data(certificate));

        signedXml.ComputeSignature();

        element.AppendChild(signedXml.GetXml());
    }

    /// <summary>
    /// Signed Xml version that is strict that the ID attribute must be exactly ID and
    /// not contains any weird fallback behaviour.
    /// </summary>
    internal class SignedXmlWithStrictIdResolution : SignedXml
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="xmlDocument">Xml document</param>
        internal SignedXmlWithStrictIdResolution(XmlDocument xmlDocument)
            : base(xmlDocument)
        { }

        /// <summary>
        /// Get Id Element, being strict
        /// </summary>
        /// <param name="document">Xml Document</param>
        /// <param name="idValue">Id value to find</param>
        /// <returns>XmlElement</returns>
        /// <exception cref="CryptographicException">If not exactly one match</exception>
        public override XmlElement GetIdElement(XmlDocument document, string idValue)
        {
            var possibleNodes = document.SelectNodes($"//*[@ID=\"{idValue}\" or @Id=\"{idValue}\" or @id=\"{idValue}\"]")!;

            if (possibleNodes.Count != 1)
            {
                throw new CryptographicException("Reference target should resolve to exactly one node");
            }

            var element = (XmlElement)possibleNodes[0]!;

            var attr = element.GetAttributeNode("ID");
            if(attr == null)
            {
                // If we don't find the ID attribute it means it matched Id or id, which is not allowed.
                throw new CryptographicException("Reference target ID attribute must be named ID with uppercase letters");
            }

            return element;
        }
    }

    /// <summary>
    /// Verifies a found Xml signature.
    /// </summary>
    /// <param name="signatureElement">The signature element to verify.</param>
    /// <param name="keys">They signing keys that can be used to verify.</param>
    /// <param name="allowedHashAlgorithms">Allowed hash algorithms. Values must be short form, 
    /// lower case e.g. sha256 to match end of algorithm identifier URI, both for
    /// signing and hashing algorithms."</param>
    /// <returns>Tuple with possibly error message, and the signing key that worked.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static (string? Error, SigningKey? WorkingKey) VerifySignature(
        this XmlElement signatureElement,
        IEnumerable<SigningKey> keys,
        IEnumerable<string> allowedHashAlgorithms)
    {
        var signedXml = new SignedXmlWithStrictIdResolution(signatureElement.OwnerDocument);

        signedXml.LoadXml(signatureElement);

        string? error = null;
        XmlElement? signedElement = null;
        SigningKey? workingKey = null;

        if (signedXml.SignedInfo.References.Count != 1)
        {
            error += "The Signature should contain exactly one reference. ";
        }
        else
        {
            // All versions of .NET prior to .NET 7 contains a bug that only lets the first signature in a
            // document be validated, we want a workaround for that. For .NET 7+ I contributed
            // this fix to the System.Security.Cryptography.Xml library.
#if !NET7_0_OR_GREATER
            FixSignatureIndex(signedXml, signatureElement);
#endif

            foreach (var key in keys)
            {
                if (signedXml.CheckSignature(key.Certificate, true))
                {
                    workingKey = key;
                }
            }

            if (workingKey == null)
            {
                if(signedXml.CheckSignatureReturningKey(out _))
                {
                    error += "Signature validated with the contained key, but that is not configured as a trusted key. ";
                }
                else
                {
                    error += "Signature didn't verify for any of the the specified keys. ";
                }
            }

            var reference = (Reference)signedXml.SignedInfo.References[0]!;

            if (reference.Uri == "")
            {
                error += "Empty reference URI (implying the whole document is signed) is not allowed in Saml2. ";
            }
            else
            {
                var id = reference.Uri.Substring(1); // Drop off the #

                XmlConvert.VerifyNCName(id);

                signedElement = signedXml.GetIdElement(signatureElement.OwnerDocument, id);

                if (signedElement == null || signedElement != signatureElement.ParentNode)
                {
                    error += "Incorrect reference on Xml Signature, the reference must be to the parent element of the signature. ";
                }
            }

            foreach (Transform transform in reference.TransformChain)
            {
                switch (transform.Algorithm)
                {
                    case SignedXml.XmlDsigEnvelopedSignatureTransformUrl:
                    case SignedXml.XmlDsigExcC14NTransformUrl:
                    case SignedXml.XmlDsigExcC14NWithCommentsTransformUrl:
                        break;
                    default:
                        error += $"Transform {transform.Algorithm} is not allowed in SAML2. ";
                        break;
                }
            }

            // The algorithm names has the form http://foo/bar/xyz#sha256
            var digestHash = reference.DigestMethod.Substring(reference.DigestMethod.LastIndexOf('#') + 1);
            if (!allowedHashAlgorithms.Contains(digestHash))
            {
                error += $"Digest algorithm {reference.DigestMethod} does not match configured [{string.Join(", ", allowedHashAlgorithms)}]. ";
            }
        }

        // The algorithm names has the form http://foo/bar/xyz#rsa-sha256
        var signingHash = signedXml.SignatureMethod.Substring(signedXml.SignatureMethod.LastIndexOf('-') + 1);
        if (!allowedHashAlgorithms.Contains(signingHash))
        {
            error += $"Signature algorithm {signedXml.SignatureMethod} does not match configured [{string.Join(", ", allowedHashAlgorithms)}]. ";
        }

        var valid = string.IsNullOrEmpty(error);

        return (error?.TrimEnd(), workingKey);
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
            if (t is XmlDsigEnvelopedSignatureTransform envelopedTransform)
            {
                signaturePosition.SetValue(envelopedTransform, correctSignaturePosition);
            }
        }
    }
}
