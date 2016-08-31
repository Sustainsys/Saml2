using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

using Kentor.AuthServices.Saml2P;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Extension methods for XmlDocument
    /// </summary>
    public static class XmlDocumentSigningExtensions
    {
        private static readonly Dictionary<MessageSigningAlgorithm, string> _algorithmToNamespaceMap = new Dictionary<MessageSigningAlgorithm, string>
        {
            { MessageSigningAlgorithm.RsaSecureHashAlgorithm1,RSASHA1},
            { MessageSigningAlgorithm.RsaSecureHashAlgorithm256,RSASHA256},
            { MessageSigningAlgorithm.RsaSecureHashAlgorithm384,RSASHA384},
            { MessageSigningAlgorithm.RsaSecureHashAlgorithm512,RSASHA512}
        };
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "RSASHA")]
        const string RSASHA1 = SignedXml.XmlDsigRSASHA1Url;//"http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "RSASHA")]
        const string RSASHA256 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "RSASHA")]
        const string RSASHA384 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "RSASHA")]
        const string RSASHA512 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";

        /// <summary>
        /// Resolve w3 org namespace from algorithm enumeration
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        public static string ToNamespace(this MessageSigningAlgorithm algorithm)
        {
            return _algorithmToNamespaceMap[algorithm];
        }

        /// <summary>
        /// Add digital signature to an xml document. 
        /// The default signing algorithm RSA SHA-256 is used in this method.
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <param name="signingCertificate"></param>
        public static void SignDocument(this XmlDocument xmlDocument, X509Certificate2 signingCertificate)
        {
            SignDocument(xmlDocument, signingCertificate, MessageSigningAlgorithm.RsaSecureHashAlgorithm256);
        }

        /// <summary>
        /// Add digital signature to an xml document. 
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <param name="signingCertificate">The signing certificate.</param>
        /// <param name="algorithm">The signing algorithm.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public static void SignDocument(this XmlDocument xmlDocument, X509Certificate2 signingCertificate, MessageSigningAlgorithm algorithm)
        {
            if (xmlDocument == null)
            {
                throw new ArgumentNullException(nameof(xmlDocument));
            }
            if (xmlDocument.DocumentElement == null)
            {
                throw new ArgumentNullException(nameof(xmlDocument), "The property DocumentElement cannot be null");
            }

            if (signingCertificate == null)
            {
                throw new ArgumentNullException(nameof(signingCertificate));
            }

            string signatureMethodNamespace = algorithm.ToNamespace();

            // Note that this will return a Basic crypto provider, with only SHA-1 support
            var key = (RSACryptoServiceProvider) signingCertificate.PrivateKey;

            using (var provider = new RSACryptoServiceProvider())
            {
                CspKeyContainerInfo enhCsp = provider.CspKeyContainerInfo;
                using (key = new RSACryptoServiceProvider(new CspParameters(enhCsp.ProviderType, enhCsp.ProviderName, key.CspKeyContainerInfo.KeyContainerName)))
                {
                    var signedXml = new SignedXml(xmlDocument);
                    signedXml.SigningKey = key;
                    signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
                    signedXml.SignedInfo.SignatureMethod = signatureMethodNamespace;

                    var reference = new Reference {Uri = "#" + xmlDocument.DocumentElement.GetAttribute("ID")};
                    reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                    reference.AddTransform(new XmlDsigExcC14NTransform());
                    signedXml.AddReference(reference);
                    signedXml.ComputeSignature();
                    xmlDocument.DocumentElement.AppendChild(xmlDocument.ImportNode(signedXml.GetXml(), true));
                }
            }
        }
    }
}