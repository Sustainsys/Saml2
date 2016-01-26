using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Linq;
using Kentor.AuthServices.Exceptions;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Extension methods for XmlDocument
    /// </summary>
    public static class XmlDocumentHelpers
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
        /// a contained enveloped signature.
        /// </summary>
        /// <param name="xmlElement">Xml Element that should be signed</param>
        /// <param name="certificate">Certificate that should validate</param>
        /// <returns>Is the signature correct?</returns>
        public static bool IsSignedBy(this XmlElement xmlElement, X509Certificate2 certificate)
        {
            if (xmlElement == null)
            {
                throw new ArgumentNullException(nameof(xmlElement));
            }

            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            var signedXml = new SignedXml(xmlElement.OwnerDocument);

            var signatureElement = xmlElement["Signature", SignedXml.XmlDsigNamespaceUrl];

            if (signatureElement == null)
            {
                return false;
            }

            signedXml.LoadXml(signatureElement);
            ValidateSignedInfo(signedXml, xmlElement);
            return signedXml.CheckSignature(certificate, true);
        }

        private static void ValidateSignedInfo(SignedXml signedXml, XmlElement xmlElement)
        {
            var reference = signedXml.SignedInfo.References.Cast<Reference>().Single();
            var id = reference.Uri.Substring(1);

            var idElement = signedXml.GetIdElement(xmlElement.OwnerDocument, id);
            
            if(idElement != xmlElement)
            {
                throw new InvalidSignatureException("Incorrect reference on Xml signature. The reference must be to the root element of the element containing the signature.");
            }
        }
    }
}
