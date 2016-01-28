using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Linq;
using Kentor.AuthServices.Exceptions;
using System.Collections.Generic;
using Kentor.AuthServices.Configuration;
using System.Reflection;
using System.IdentityModel.Tokens;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Extension methods and helpers for XmlDocument/XmlElement etc.
    /// </summary>
    public static class XmlHelpers
    {
        /// <summary>
        /// Sign an xml document with the supplied cert.
        /// </summary>
        /// <param name="xmlDocument">XmlDocument to be signed. The signature is
        /// added as a node in the document, right after the Issuer node.</param>
        /// <param name="cert">Certificate to use when signing.</param>
        public static void Sign(this XmlDocument xmlDocument, X509Certificate2 cert)
        {
            Sign(xmlDocument, cert, false);
        }

        /// <summary>
        /// Parse XML data from a string.
        /// </summary>
        /// <param name="source">Source string to load</param>
        /// <returns>Xml document</returns>
        public static XmlDocument FromString(string source)
        {
            var xmlDoc = new XmlDocument()
            {
                PreserveWhitespace = true
            };

            xmlDoc.LoadXml(source);

            return xmlDoc;
        }

        /// <summary>
        /// Remove the attribute with the given name from the collection.
        /// </summary>
        /// <param name="attributes">Attribute collection.</param>
        /// <param name="attributeName">Name of attribute to remove.</param>
        public static void Remove(this XmlAttributeCollection attributes, string attributeName)
        {
            if(attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            if(attributeName == null)
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            var attribute = attributes[attributeName];

            attributes.Remove(attribute);
        }

        /// <summary>
        /// Remove the child xml element with the specified name.
        /// </summary>
        /// <param name="xmlElement">Parent</param>
        /// <param name="name">Name of child</param>
        /// <param name="ns">Namespace of child</param>
        public static void RemoveChild(this XmlElement xmlElement, string name, string ns)
        {
            if(xmlElement == null)
            {
                throw new ArgumentNullException(nameof(xmlElement));
            }

            if(name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if(ns == null)
            {
                throw new ArgumentNullException(nameof(ns));
            }

            var toRemove = xmlElement[name, ns];
            xmlElement.RemoveChild(toRemove);
        }

        /// <summary>
        /// Sign an xml document with the supplied cert.
        /// </summary>
        /// <param name="xmlDocument">XmlDocument to be signed. The signature is
        /// added as a node in the document, right after the Issuer node.</param>
        /// <param name="cert">Certificate to use when signing.</param>
        /// <param name="includeKeyInfo">Include public key in signed output.</param>
        public static void Sign(this XmlDocument xmlDocument, X509Certificate2 cert, bool includeKeyInfo)
        {
            if(xmlDocument == null)
            {
                throw new ArgumentNullException(nameof(xmlDocument));
            }

            xmlDocument.DocumentElement.Sign(cert, includeKeyInfo);
        }

        /// <summary>
        /// Sign an xml element with the supplied cert.
        /// </summary>
        /// <param name="xmlElement">xmlElement to be signed. The signature is
        /// added as a node in the document, right after the Issuer node.</param>
        /// <param name="cert">Certificate to use when signing.</param>
        /// <param name="includeKeyInfo">Include public key in signed output.</param>
        public static void Sign(this XmlElement xmlElement, X509Certificate2 cert, bool includeKeyInfo)
        {
            if (xmlElement == null)
            {
                throw new ArgumentNullException(nameof(xmlElement));
            }

            if (cert == null)
            {
                throw new ArgumentNullException(nameof(cert));
            }

            var signedXml = new SignedXml(xmlElement.OwnerDocument);

            // The transform XmlDsigExcC14NTransform and canonicalization method XmlDsigExcC14NTransformUrl is important for partially signed XML files
            // see: http://msdn.microsoft.com/en-us/library/system.security.cryptography.xml.signedxml.xmldsigexcc14ntransformurl(v=vs.110).aspx
            // The reference URI has to be set correctly to avoid assertion injections
            // For both, the ID/Reference and the Transform/Canonicalization see as well: 
            // https://www.oasis-open.org/committees/download.php/35711/sstc-saml-core-errata-2.0-wd-06-diff.pdf section 5.4.2 and 5.4.3

            signedXml.SigningKey = (RSACryptoServiceProvider)cert.PrivateKey;
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            var reference = new Reference { Uri = "#" + xmlElement.GetAttribute("ID") };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());

            signedXml.AddReference(reference);
            signedXml.ComputeSignature();

            if (includeKeyInfo)
            {
                var keyInfo = new KeyInfo();
                keyInfo.AddClause(new KeyInfoX509Data(cert));
                signedXml.KeyInfo = keyInfo;
            }

            xmlElement.InsertAfter(
                xmlElement.OwnerDocument.ImportNode(signedXml.GetXml(), true),
                xmlElement["Issuer", Saml2Namespaces.Saml2Name]);
        }

        /// <summary>
        /// Checks if an xml element is signed by the given certificate, through
        /// a contained enveloped signature. Helper for tests. Production
        /// code always should handle multiple possible signing keys.
        /// </summary>
        /// <param name="xmlElement">Xml Element that should be signed</param>
        /// <param name="certificate">Certificate that should validate</param>
        /// <returns>Is the signature correct?</returns>
        internal static bool IsSignedBy(this XmlElement xmlElement, X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            return xmlElement.IsSignedByAny(
                Enumerable.Repeat(new X509RawDataKeyIdentifierClause(certificate), 1));
        }

        /// <summary>
        /// Checks if an xml element is signed by the given certificate, through
        /// a contained enveloped signature.
        /// </summary>
        /// <param name="xmlElement">Xml Element that should be signed</param>
        /// <param name="signingKeys">Signing keys to test, one should validate.</param>
        /// <returns>True on correct signature, false on missing signature</returns>
        /// <exception cref="InvalidSignatureException">If the data has
        /// been tampered with or is not valid according to the SAML spec.</exception>
        public static bool IsSignedByAny(
            this XmlElement xmlElement, 
            IEnumerable<SecurityKeyIdentifierClause> signingKeys)
        {
            if (xmlElement == null)
            {
                throw new ArgumentNullException(nameof(xmlElement));
            }

            var signedXml = new SignedXml(xmlElement);

            var signatureElement = xmlElement["Signature", SignedXml.XmlDsigNamespaceUrl];

            if (signatureElement == null)
            {
                return false;
            }

            signedXml.LoadXml(signatureElement);
            ValidateSignedInfo(signedXml, xmlElement);
            VerifySignature(signingKeys, signedXml, signatureElement);

            return true;
        }

        private static void VerifySignature(
            IEnumerable<SecurityKeyIdentifierClause> signingKeys,
            SignedXml signedXml,
            XmlElement signatureElement)
        {
            FixSignatureIndex(signedXml, signatureElement);

            try
            {
                if (!signingKeys
                    .Select(c => ((AsymmetricSecurityKey)c.CreateKey())
                    .GetAsymmetricAlgorithm(SignedXml.XmlDsigRSASHA1Url, false))
                    .Any(signedXml.CheckSignature))
                {
                    var containedKey = signedXml.Signature.KeyInfo.OfType<KeyInfoX509Data>()
                        .SingleOrDefault()?.Certificates.OfType<X509Certificate2>()
                        .SingleOrDefault();

                    if (containedKey != null && signedXml.CheckSignature(containedKey, true))
                    {
                        throw new InvalidSignatureException("The signature verified correctly with the key contained in the signature, but that key is not trusted.");
                    }

                    throw new InvalidSignatureException("Signature didn't verify. Have the contents been tampered with?");
                }
            }
            catch (CryptographicException)
            {
                if (signedXml.SignatureMethod == Options.RsaSha256Namespace && CryptoConfig.CreateFromName(signedXml.SignatureMethod) == null)
                {
                    throw new InvalidSignatureException("SHA256 signatures require the algorithm to be registered at the process level. Call Kentor.AuthServices.Configuration.Options.GlobalEnableSha256XmlSignatures() on startup to register.");
                }
                else
                {
                    throw;
                }
            }
        }

        static readonly PropertyInfo signaturePosition = typeof(XmlDsigEnvelopedSignatureTransform)
            .GetProperty("SignaturePosition", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Workaround for a bug in Reference.LoadXml incorrectly counting index
        /// of signature from the start of the document, not from the start of
        /// the element. Reported to Microsoft at
        /// https://connect.microsoft.com/VisualStudio/feedback/details/2288620
        /// </summary>
        /// <param name="signedXml">SignedXml</param>
        /// <param name="signatureElement">Signature element.</param>
        private static void FixSignatureIndex(SignedXml signedXml, XmlElement signatureElement)
        {
            Transform transform = null;
            foreach(var t in ((Reference)signedXml.SignedInfo.References[0]).TransformChain)
            {
                var envelopeTransform = t as XmlDsigEnvelopedSignatureTransform;
                if(envelopeTransform != null)
                {
                    transform = envelopeTransform;
                }
            }

            if(signaturePosition != null)
            {
                var nsm = new XmlNamespaceManager(signatureElement.OwnerDocument.NameTable);
                nsm.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);

                var signaturesInParent = signatureElement.ParentNode.SelectNodes(".//ds:Signature", nsm);

                int correctSignaturePosition = 0;
                for (int i = 0; i < signaturesInParent.Count; i++)
                {
                    if (signaturesInParent[i] == signatureElement)
                    {
                        correctSignaturePosition = i + 1;
                    }
                }

                signaturePosition.SetValue(transform, correctSignaturePosition);
            }
        }

        private static readonly string[] allowedTransforms = new string[]
            {
            SignedXml.XmlDsigEnvelopedSignatureTransformUrl,
            SignedXml.XmlDsigExcC14NTransformUrl,
            SignedXml.XmlDsigExcC14NWithCommentsTransformUrl
            };

        private static void ValidateSignedInfo(SignedXml signedXml, XmlElement xmlElement)
        {
            if(signedXml.SignedInfo.References.Count == 0)
            {
                throw new InvalidSignatureException("No reference found in Xml signature, it doesn't validate the Xml data.");
            }

            if(signedXml.SignedInfo.References.Count != 1)
            {
                throw new InvalidSignatureException("Multiple references for Xml signatures are not allowed.");
            }

            var reference = (Reference)signedXml.SignedInfo.References[0];
            var id = reference.Uri.Substring(1);

            var idElement = signedXml.GetIdElement(xmlElement.OwnerDocument, id);
            
            if(idElement != xmlElement)
            {
                throw new InvalidSignatureException("Incorrect reference on Xml signature. The reference must be to the root element of the element containing the signature.");
            }

            foreach (Transform transform in reference.TransformChain)
            {
                if (!allowedTransforms.Contains(transform.Algorithm))
                {
                    throw new InvalidSignatureException(
                        "Transform \"" + transform.Algorithm + "\" found in Xml signature SHOULD NOT be used with SAML2.");
                }
            }
        }
    }
}
