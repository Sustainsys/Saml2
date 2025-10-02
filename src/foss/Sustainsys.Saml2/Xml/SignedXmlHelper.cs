// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Xml;

/// <summary>
/// Helpers for SignedXml
/// </summary>
public static class SignedXmlHelper
{
    /// <summary>
    /// Adds an enveloped signature to the node.
    /// </summary>
    /// <param name="element">Element to sign</param>
    /// <param name="certificate">Certificate to use to sign</param>
    public static void Sign(
        this XmlElement element,
        X509Certificate2 certificate)
    {
        var signedXml = CreateSignedXml(element, certificate);

        element.AppendChild(signedXml.GetXml());
    }

    /// <summary>
    /// Adds an enveloped signature to the node.
    /// </summary>
    /// <param name="element">Element to sign</param>
    /// <param name="certificate">Certificate to use to sign</param>
    /// <param name="insertAfter">Insert the signature after this node.</param>
    public static void Sign(
        this XmlElement element,
        X509Certificate2 certificate,
        XmlNode insertAfter)
    {
        ArgumentNullException.ThrowIfNull(insertAfter);

        var signedXml = CreateSignedXml(element, certificate);

        element.InsertAfter(signedXml.GetXml(), insertAfter);
    }

    private static SignedXml CreateSignedXml(XmlElement element, X509Certificate2 certificate)
    {
        var signedXml = new SignedXml(element.OwnerDocument)
        {
            SigningKey = certificate.PublicKey.Oid.FriendlyName switch
            {
                "RSA" => certificate.GetRSAPrivateKey(),
                _ => throw new NotImplementedException(),
            }
        };

        var id = element.Attributes!["ID"]?.Value;

        var reference = new Reference($"#{id}");
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigExcC14NTransform());
        signedXml.AddReference(reference);

        signedXml.KeyInfo.AddClause(new KeyInfoX509Data(certificate));

        signedXml.ComputeSignature();
        return signedXml;
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
        public override XmlElement GetIdElement(XmlDocument? document, string idValue)
        {
            ArgumentNullException.ThrowIfNull(document);

            XmlConvert.VerifyNCName(idValue);

            var possibleNodes = document.SelectNodes($"//*[@ID=\"{idValue}\" or @Id=\"{idValue}\" or @id=\"{idValue}\"]")!;

            if (possibleNodes.Count != 1)
            {
                throw new CryptographicException("Reference target should resolve to exactly one node");
            }

            var element = (XmlElement)possibleNodes[0]!;

            // If we don't find the ID attribute it means it matched Id or id, which is not allowed.
            _ = element.GetAttributeNode("ID")
                ?? throw new CryptographicException("Reference target ID attribute must be named ID with uppercase letters");

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
    public static (string? Error, SigningKey? WorkingKey) VerifySignature(
        this XmlElement signatureElement,
        IEnumerable<SigningKey> keys,
        IEnumerable<string> allowedHashAlgorithms)
    {
        var signedXml = new SignedXmlWithStrictIdResolution(signatureElement.OwnerDocument);

        signedXml.LoadXml(signatureElement);

        string? error = null;
        SigningKey? workingKey = null;

        if (signedXml.SignedInfo!.References.Count != 1)
        {
            error += "The Signature should contain exactly one reference. ";
        }
        else
        {
            foreach (var key in keys)
            {
                if (key.Certificate == null)
                {
                    throw new InvalidOperationException("Signing key certificate cannot be null");
                }

                if (signedXml.CheckSignature(key.Certificate, true))
                {
                    workingKey = key;
                }
            }

            if (workingKey == null)
            {
                if (signedXml.CheckSignatureReturningKey(out _))
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
                var id = reference.Uri![1..]; // Drop off the #

                var signedElement = signedXml.GetIdElement(signatureElement.OwnerDocument, id);

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
            var digestHash = reference.DigestMethod[(reference.DigestMethod.LastIndexOf('#') + 1)..];
            if (!allowedHashAlgorithms.Contains(digestHash))
            {
                error += $"Digest algorithm {reference.DigestMethod} does not match configured [{string.Join(", ", allowedHashAlgorithms)}]. ";
            }
        }

        // The algorithm names has the form http://foo/bar/xyz#rsa-sha256
        var signingHash = signedXml.SignatureMethod![(signedXml.SignatureMethod!.LastIndexOf('-') + 1)..];
        if (!allowedHashAlgorithms.Contains(signingHash))
        {
            var allowed = string.Join(", ", allowedHashAlgorithms);
            error += $"Signature algorithm {signedXml.SignatureMethod} does not match configured [{allowed}]. ";
        }

        return (error?.TrimEnd(), workingKey);
    }
}