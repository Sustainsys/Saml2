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
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using System.IO;
using Kentor.AuthServices.Internal;

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
        /// Sign an xml document with the supplied cert.
        /// </summary>
        /// <param name="xmlDocument">XmlDocument to be signed. The signature is
        /// added as a node in the document, right after the Issuer node.</param>
        /// <param name="cert">Certificate to use when signing.</param>
        /// <param name="includeKeyInfo">Include public key in signed output.</param>
        /// <param name="signingAlgorithm">Uri of signing algorithm to use.</param>
        public static void Sign(
            this XmlDocument xmlDocument,
            X509Certificate2 cert,
            bool includeKeyInfo,
            string signingAlgorithm)
        {
            if (xmlDocument == null)
            {
                throw new ArgumentNullException(nameof(xmlDocument));
            }

            xmlDocument.DocumentElement.Sign(cert, includeKeyInfo, signingAlgorithm);
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
            xmlElement.Sign(cert, includeKeyInfo, GetDefaultSigningAlgorithmName());
        }

        /// <summary>
        /// Sign an xml element with the supplied cert.
        /// </summary>
        /// <param name="xmlElement">xmlElement to be signed. The signature is
        /// added as a node in the document, right after the Issuer node.</param>
        /// <param name="cert">Certificate to use when signing.</param>
        /// <param name="includeKeyInfo">Include public key in signed output.</param>
        /// <param name="signingAlgorithm">The signing algorithm to use.</param>
        public static void Sign(
            this XmlElement xmlElement,
            X509Certificate2 cert,
            bool includeKeyInfo,
            string signingAlgorithm)
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

            signedXml.SigningKey = ((RSACryptoServiceProvider)cert.PrivateKey)
                .GetSha256EnabledRSACryptoServiceProvider();
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.SignedInfo.SignatureMethod = signingAlgorithm;

            var reference = new Reference
            {
                Uri = "#" + xmlElement.GetAttribute("ID"),
                DigestMethod = GetCorrespondingDigestAlgorithm(signingAlgorithm)
            };
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
        /// a contained enveloped signature.
        /// </summary>
        /// <param name="xmlElement">Xml Element that should be signed</param>
        /// <param name="signingKeys">Signing keys to test, one should validate.</param>
        /// <param name="validateCertificate">Should the certificate be validated too?</param>
        /// <param name="minimumSigningAlgorithm">The mininum signing algorithm
        /// strength allowed.</param>
        /// <returns>True on correct signature, false on missing signature</returns>
        /// <exception cref="InvalidSignatureException">If the data has
        /// been tampered with or is not valid according to the SAML spec.</exception>
        public static bool IsSignedByAny(
            this XmlElement xmlElement, 
            IEnumerable<SecurityKeyIdentifierClause> signingKeys,
            bool validateCertificate,
            string minimumSigningAlgorithm)
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
            ValidateSignedInfo(signedXml, xmlElement, minimumSigningAlgorithm);
            VerifySignature(signingKeys, signedXml, signatureElement, validateCertificate);

            return true;
        }

        private static void VerifySignature(
            IEnumerable<SecurityKeyIdentifierClause> signingKeys,
            SignedXml signedXml,
            XmlElement signatureElement,
            bool validateCertificate)
        {
            FixSignatureIndex(signedXml, signatureElement);

            CheckSha256Support(signedXml);

            foreach (var keyIdentifier in signingKeys)
            {
                var key = ((AsymmetricSecurityKey)keyIdentifier.CreateKey())
                .GetAsymmetricAlgorithm(SignedXml.XmlDsigRSASHA1Url, false);

                if (signedXml.CheckSignature(key))
                {
                    ValidateCertificate(validateCertificate, keyIdentifier);
                    return;
                }
            }
            var containedKey = signedXml.Signature.KeyInfo.OfType<KeyInfoX509Data>()
                .SingleOrDefault()?.Certificates.OfType<X509Certificate2>()
                .SingleOrDefault();

            if (containedKey != null && signedXml.CheckSignature(containedKey, true))
            {
                throw new InvalidSignatureException("The signature verified correctly with the key contained in the signature, but that key is not trusted.");
            }

            throw new InvalidSignatureException("Signature didn't verify. Have the contents been tampered with?");
        }

        private static readonly Lazy<object> rsaSha256Algorithm = 
            new Lazy<object>(() => CryptoConfig.CreateFromName(Options.RsaSha256Uri));

        [ExcludeFromCodeCoverage]
        private static void CheckSha256Support(SignedXml signedXml)
        {
            if (signedXml.SignatureMethod == Options.RsaSha256Uri
                && rsaSha256Algorithm.Value == null)
            {
                throw new InvalidSignatureException("SHA256 signatures require the algorithm to be registered at the process level. Upgrade to .Net 4.6.2 or call Kentor.AuthServices.Configuration.Options.GlobalEnableSha256XmlSignatures() on startup to register.");
            }
        }

        // Splitting up in several methods to set ExcludeFromCodeCoverage on
        // as small part of the code as possible. To actually have a test
        // case that passes with a valid cert would require a signature
        // that is created by a long lived official cert or the tests would
        // mysteriously start to fail when the cert expired.
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ValidateCertificates")]
        [ExcludeFromCodeCoverage]
        private static void ValidateCertificate(bool validateCertificate, SecurityKeyIdentifierClause keyIdentifier)
        {
            if (validateCertificate)
            {
                var rawCert = keyIdentifier as X509RawDataKeyIdentifierClause;
                if (rawCert == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                        "Certificate validation enabled, but the signing key identifier is of type {0} which cannot be validated as a certificate.",
                        keyIdentifier.GetType().Name));
                }

                if (!new X509Certificate2(rawCert.GetX509RawData()).Verify())
                {
                    throw new InvalidSignatureException("The signature was valid, but the verification of the certificate failed. Is it expired or revoked? Are you sure you really want to enable ValidateCertificates (it's normally not needed)?");
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

        private static void ValidateSignedInfo(
            SignedXml signedXml,
            XmlElement xmlElement,
            string minIncomingSignatureAlgorithm)
        {
            var signatureMethod = signedXml.SignedInfo.SignatureMethod;
            ValidateSignatureMethodStrength(minIncomingSignatureAlgorithm, signatureMethod);

            ValidateReference(signedXml, xmlElement, GetCorrespondingDigestAlgorithm(minIncomingSignatureAlgorithm));
        }

        /// <summary>
        /// Check if the signature method is at least as strong as the mininum one.
        /// </summary>
        /// <param name="minIncomingSignatureAlgorithm"></param>
        /// <param name="signatureMethod"></param>
        /// <exception cref="InvalidSignatureException">If the signaturemethod is too weak.</exception>
        public static void ValidateSignatureMethodStrength(
            string minIncomingSignatureAlgorithm,
            string signatureMethod)
        {
            if (!KnownSigningAlgorithms.SkipWhile(a => a != minIncomingSignatureAlgorithm)
                .Contains(signatureMethod))
            {
                throw new InvalidSignatureException(
                    "The signing algorithm " + signatureMethod +
                    " is weaker than the minimum accepted " + minIncomingSignatureAlgorithm + ".");
            }
        }

        private static void ValidateReference(
            SignedXml signedXml,
            XmlElement xmlElement,
            string mininumDigestAlgorithm)
        {
            if (signedXml.SignedInfo.References.Count == 0)
            {
                throw new InvalidSignatureException("No reference found in Xml signature, it doesn't validate the Xml data.");
            }

            if (signedXml.SignedInfo.References.Count != 1)
            {
                throw new InvalidSignatureException("Multiple references for Xml signatures are not allowed.");
            }

            var reference = (Reference)signedXml.SignedInfo.References[0];
            var id = reference.Uri.Substring(1);

            var idElement = signedXml.GetIdElement(xmlElement.OwnerDocument, id);

            if (idElement != xmlElement)
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

            if(!DigestAlgorithms.SkipWhile(a => a != mininumDigestAlgorithm)
                .Contains(reference.DigestMethod))
            {
                throw new InvalidSignatureException("The digest method " + reference.DigestMethod
                    + " is weaker than the minimum accepted " + mininumDigestAlgorithm + ".");
            }
        }

        internal static XElement AddAttributeIfNotNullOrEmpty(this XElement xElement, XName attribute, object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                xElement.Add(new XAttribute(attribute, value));
            }
            return xElement;
        }

        internal static string GetValueIfNotNull(this XmlAttribute xmlAttribute)
        {
            if (xmlAttribute == null)
            {
                return null;
            }
            return xmlAttribute.Value;
        }

        internal static string GetTrimmedTextIfNotNull(this XmlElement xmlElement)
        {
            if (xmlElement == null)
            {
                return null;
            }

            return xmlElement.InnerText.Trim();
        }

        internal static XmlElement StartElement(this XmlNode parent, string name, Uri namespaceUri)
        {
            var xmlElement = parent.GetOwnerDoc().CreateElement(name, namespaceUri.OriginalString);
            parent.AppendChild(xmlElement);
            return xmlElement;
        }

        private static XmlDocument GetOwnerDoc(this XmlNode node)
        {
            var doc = node as XmlDocument;
            if(doc != null)
            {
                return doc;
            }

            return node.OwnerDocument;
        }

        internal static XmlElement AddAttribute(this XmlElement parent, string name, string value)
        {
            parent.SetAttribute(name, value);

            return parent;
        }

        internal static XmlElement AddAttributeIfNotNull(this XmlElement parent, string name, object value)
        {
            if(value != null)
            {
                parent.SetAttribute(name, value.ToString());
            }
            return parent;
        }

        internal static XmlElement If(this XmlElement parent, bool condition, Action<XmlElement> action)
        {
            if (condition)
            {
                action(parent);
            }

            return parent;
        }

        internal static XmlElement AddElement(this XmlElement parent, string name, Uri namespaceUri, string content)
        {
            parent.StartElement(name, namespaceUri)
                .SetInnerText(content);

            return parent;
        }

        internal static XmlElement SetInnerText(this XmlElement parent, string content)
        {
            parent.InnerText = content;
            return parent;
        }

        /// <summary>
        /// Pretty an xml element.
        /// </summary>
        /// <param name="xml">Xml to pretty print.</param>
        /// <returns>Nicely indented and readable data.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "I don't care, the StringWriter contains no references to unmanaged resources")]
        public static string PrettyPrint(this XmlElement xml)
        {
            if(xml == null)
            {
                throw new ArgumentNullException(nameof(xml));
            }

            // Based on http://stackoverflow.com/a/1123731/280222
            var strWriter = new StringWriter(CultureInfo.InvariantCulture);
            using (var xmlWriter = new XmlTextWriter(strWriter))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xml.ParentNode.WriteContentTo(xmlWriter);
                xmlWriter.Flush();
                return strWriter.ToString();
            }
        }

        /// <summary>
        /// Store a list of signing algorithms that are available in SignedXml.
        /// This needs to be done through reflection, to keep the library
        /// targetting lowest supported .NET version, while still getting
        /// access to new algorithms if the hosting application targets a
        /// later version.
        /// </summary>
        internal static readonly IEnumerable<string> KnownSigningAlgorithms =
            typeof(SignedXml).GetFields()
            .Where(f => f.Name.StartsWith("XmlDsigRSASHA", StringComparison.Ordinal))
            .Select(f => (string)f.GetRawConstantValue())
            .OrderBy(f => f)
            .ToList();

        internal static string GetFullSigningAlgorithmName(string shortName)
        {
            return string.IsNullOrEmpty(shortName) ?
                GetDefaultSigningAlgorithmName()
                : KnownSigningAlgorithms.Single(
                a => a.EndsWith(shortName, StringComparison.OrdinalIgnoreCase));
        }

        // Can't test the fallback behaviour on a machine that has a modern
        // framework installed.
        [ExcludeFromCodeCoverage]
        internal static string GetDefaultSigningAlgorithmName()
        {
            var rsaSha256Name = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
            if (KnownSigningAlgorithms.Contains(rsaSha256Name))
            {
                return rsaSha256Name;
            }
            return SignedXml.XmlDsigRSASHA1Url;
        }

        internal static readonly IEnumerable<string> DigestAlgorithms =
            typeof(SignedXml).GetFields()
            .Where(f => f.Name.StartsWith("XmlDsigSHA", StringComparison.Ordinal))
            .Select(f => (string)f.GetRawConstantValue())
            .OrderBy(f => f)
            .ToList();

        internal static string GetCorrespondingDigestAlgorithm(string signingAlgorithm)
        {
            var matchPattern = signingAlgorithm.Substring(signingAlgorithm.LastIndexOf('-') + 1);

            return DigestAlgorithms.Single(a => a.EndsWith(
                matchPattern,
                StringComparison.Ordinal));
        }
    }
}
