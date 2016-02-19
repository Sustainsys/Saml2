using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Kentor.AuthServices.Internal
{
    static class XmlHelpers
    {
        public static XElement AddAttributeIfNotNullOrEmpty(this XElement xElement, XName attribute, object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                xElement.Add(new XAttribute(attribute, value));
            }
            return xElement;
        }

        public static string GetValueIfNotNull(this XmlAttribute xmlAttribute)
        {
            if (xmlAttribute == null)
            {
                return null;
            }
            return xmlAttribute.Value;
        }

        public static string GetTrimmedTextIfNotNull(this XmlElement xmlElement)
        {
            if (xmlElement == null)
            {
                return null;
            }

            return xmlElement.InnerText.Trim();
        }

        public static XmlDocument ToXmlDocument(this XElement xDocument)
        {
            var xmlDocument = new XmlDocument();
            using(var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }
        public static XElement ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                var xDoc =  XDocument.Load(nodeReader);
                 return XElement.Parse(xDoc.ToString());
            }
        }

/*
        /// <summary>
        /// Sign an xml document with the supplied cert.
        /// </summary>
        /// <param name="xmlDocument">XmlDocument to be signed. The signature is
        /// added as a node in the document, right after the Issuer node.</param>
        /// <param name="cert">Certificate to use when signing.</param>
        /// <param name="includeKeyInfo">Include public key in signed output.</param>
        public static void Sign(this XElement xmlDocument, X509Certificate2 cert, bool includeKeyInfo)
        {
            if (xmlDocument == null)
            {
                throw new ArgumentNullException("xmlDocument");
            }

            if (cert == null)
            {
                throw new ArgumentNullException("cert");
            }

            var signedXml = new SignedXml(xmlDocument);

            // The transform XmlDsigExcC14NTransform and canonicalization method XmlDsigExcC14NTransformUrl is important for partially signed XML files
            // see: http://msdn.microsoft.com/en-us/library/system.security.cryptography.xml.signedxml.xmldsigexcc14ntransformurl(v=vs.110).aspx
            // The reference URI has to be set correctly to avoid assertion injections
            // For both, the ID/Reference and the Transform/Canonicalization see as well:
            // https://www.oasis-open.org/committees/download.php/35711/sstc-saml-core-errata-2.0-wd-06-diff.pdf section 5.4.2 and 5.4.3

            signedXml.SigningKey = (RSACryptoServiceProvider)cert.PrivateKey;
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            var reference = new Reference { Uri = "#" + xmlDocument.DocumentElement.GetAttribute("ID") };
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

            xmlDocument.DocumentElement.InsertAfter(
                xmlDocument.ImportNode(signedXml.GetXml(), true),
                xmlDocument.DocumentElement["Issuer", Saml2Namespaces.Saml2Name]);
        }
        */
    }
}
