using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Extension methods for XmlDocument
    /// </summary>
    public static class XmlDocumentExtensions
    {
        /// <summary>
        /// Sign an xml document with the supplied cert.
        /// </summary>
        /// <param name="xmlDocument">XmlDocument to be signed. The signature is
        /// added as a node in the document.</param>
        /// <param name="cert">Certificate to use when signing.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public static void Sign(this XmlDocument xmlDocument, X509Certificate2 cert)
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
            // For both, the ID/Reference and the Transform/Canonicalization see as well: http://docs.oasis-open.org/security/saml/v2.0/saml-core-2.0-os.pdf

            signedXml.SigningKey = (RSACryptoServiceProvider)cert.PrivateKey;
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            var reference = new Reference { Uri = "#" + xmlDocument.DocumentElement.GetAttribute("ID") };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());

            signedXml.AddReference(reference);
            signedXml.ComputeSignature();

            xmlDocument.DocumentElement.AppendChild(xmlDocument.ImportNode(signedXml.GetXml(), true));
        }
    }
}
