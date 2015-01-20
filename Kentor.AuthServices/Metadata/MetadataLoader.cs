using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.IdentityModel.Selectors;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Kentor.AuthServices.Metadata
{
    /// <summary>
    /// Helper for loading SAML2 metadata
    /// </summary>
    public static class MetadataLoader
    {
        /// <summary>
        /// Convenience method that loads idp metadata with no provided specific signature key and if
        /// a metadata signature is present and the key is provided through the metadata the key will be validated 
        /// through the local certificate store. 
        /// </summary>
        /// <param name="metadataUrl">Url to metadata</param>
        /// <returns>EntityDescriptor containing the metadata.</returns>
        public static ExtendedEntityDescriptor LoadIdp(Uri metadataUrl)
        {
            return LoadIdp(metadataUrl, null, SignatureValidationMethod.Default);
        }


        /// <summary>
        /// Load and parse metadata.
        /// </summary>
        /// <param name="metadataUrl">Url to metadata</param>
        /// <param name="signingCertificate">The key that is provided to check the signature of the metadata.</param>
        /// <param name="signatureValidationMethod">Determines the way the metadata signatures will be handled.</param>
        /// <returns>EntityDescriptor containing metadata</returns>
        public static ExtendedEntityDescriptor LoadIdp(Uri metadataUrl, X509Certificate2 signingCertificate, SignatureValidationMethod signatureValidationMethod)
        {
            if (metadataUrl == null)
            {
                throw new ArgumentNullException("metadataUrl");
            }

            return (ExtendedEntityDescriptor)Load(metadataUrl, signingCertificate, signatureValidationMethod);
        }

        private static MetadataBase Load(Uri metadataUrl, X509Certificate2 signingCertificate, SignatureValidationMethod signatureValidationMethod)
        {
            using (var client = new WebClient())
            using (var stream = client.OpenRead(metadataUrl.ToString()))
            {
                return Load(stream, signingCertificate, signatureValidationMethod);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        internal static MetadataBase Load(Stream metadataStream, X509Certificate2 signingCertificate, SignatureValidationMethod signatureValidationMethod)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                metadataStream.CopyTo(memStream);
                if (signatureValidationMethod != SignatureValidationMethod.Skip)
                {
                    AsymmetricAlgorithm publicKey = null;
                    if (signingCertificate != null && signingCertificate.PublicKey != null)
                    {
                        publicKey = signingCertificate.PublicKey.Key;
                    }
                    ValidateStream(memStream, publicKey, signatureValidationMethod);
                }
                memStream.Seek(0, SeekOrigin.Begin);

                var serializer = ExtendedMetadataSerializer.ReaderInstance;
                using (var reader = XmlDictionaryReader.CreateTextReader(memStream, XmlDictionaryReaderQuotas.Max))
                {
                    // If the signingCertificate is null we are not provided a certificate to validate against. So we strip the signature.
                    // Otherwise it will go to the certificate store to fetch the certificate.
                    if (signatureValidationMethod == SignatureValidationMethod.Skip)
                    {
                        // Filter out the signature from the metadata, as the built in MetadataSerializer
                        // doesn't handle the http://www.w3.org/2000/09/xmldsig# which is allowed (and for SAMLv1
                        // even recommended).
                        using (var filter = new XmlFilteringReader("http://www.w3.org/2000/09/xmldsig#", "Signature", reader))
                        {
                            return serializer.ReadMetadata(filter);
                        }
                    }

                    if (signingCertificate != null)
                    {
                        serializer.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
                        serializer.CertificateValidator = new X509FromFileCertificateValidator(signingCertificate);
                    }

                    return serializer.ReadMetadata(reader);
                }
            }
        }

        private static void ValidateStream(Stream stream, AsymmetricAlgorithm signingKey, SignatureValidationMethod signatureValidationMethod)
        {
            stream.Seek(0, SeekOrigin.Begin);

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(stream);
            var metadataSignature = xmlDocument.DocumentElement["Signature", SignedXml.XmlDsigNamespaceUrl];

            
            ValidationHelper helper = new ValidationHelper(new Saml2MetadataExceptionGenerator());

            bool unsignedElements = false;
            if (metadataSignature != null)
            {
                helper.CheckSignature(xmlDocument.DocumentElement, signingKey, GetCertificateKeyFromSignature);
            }
            else
            {
                var descriptors = FindEntityElements(xmlDocument);
                if (descriptors.Count() == 0)
                {
                    // The document root must have been unsigned and we have no
                    // other elements that can be signed. 
                    unsignedElements = true;
                }
                // Check the entities individually for signatures.
                foreach (var entityDescriptor in FindEntityElements(xmlDocument))
                {
                    metadataSignature = entityDescriptor["Signature", SignedXml.XmlDsigNamespaceUrl];
                    if (metadataSignature != null)
                    {
                        helper.CheckSignature(xmlDocument.DocumentElement, signingKey, GetCertificateKeyFromSignature);
                    }
                    else
                    {
                        unsignedElements = true;
                    }
                }
            }

            if (signatureValidationMethod == SignatureValidationMethod.Demand && unsignedElements)
            {
                throw new MetadataFailedValidationException("A signature was demanded but the metadata was delivered unsigned.");
            }
        }

        private static AsymmetricAlgorithm GetCertificateKeyFromSignature(Signature signature)
        {
            if (signature.KeyInfo.Count == 0)
            {
                throw new MetadataFailedValidationException("No key was provided and there was no certificate in the message. Cannot validate signature.");
            }

            var x509Certificate = signature.KeyInfo.OfType<KeyInfoX509Data>().First().Certificates.OfType<X509Certificate2>().First();

            return x509Certificate.PublicKey.Key;
            
        }

        private static IEnumerable<XmlElement> FindEntityElements(XmlDocument document)
        {
            return document.DocumentElement.ChildNodes.Cast<XmlNode>()
                        .Where(node => node.NodeType == XmlNodeType.Element).Cast<XmlElement>()
                        .Where(xe => xe.LocalName == "EntityDescriptor" && xe.NamespaceURI == Saml2Namespaces.Saml2Metadata);
        }

        /// <summary>
        /// Load and parse metadata for a federation. This is a convenience method that loads idp metadata with no 
        /// provided specific signature key and if a metadata signature is present and the key is provided through 
        /// the metadata the key will be validated through the local certificate store.  
        /// </summary>
        /// <param name="metadataUrl">Url to metadata</param>
        /// <returns>An <see cref="ExtendedEntitiesDescriptor"/> that represents the federation metadata.</returns>
        public static ExtendedEntitiesDescriptor LoadFederation(Uri metadataUrl)
        {
            return (ExtendedEntitiesDescriptor)LoadFederation(metadataUrl, null, SignatureValidationMethod.Default);
        }

        /// <summary>
        /// Load and parse metadata for a federation.
        /// </summary>
        /// <param name="metadataUrl">Url to metadata</param>
        /// <param name="signingCertificate">An externally loaded certificate that will validate the metadata.</param>
        /// <param name="demandSignature">If set to true, federation data will not be loaded if there is no signature.</param>
        /// <returns>An <see cref="ExtendedEntitiesDescriptor"/> that represents the federation metadata.</returns>
        public static ExtendedEntitiesDescriptor LoadFederation(Uri metadataUrl, X509Certificate2 signingCertificate, SignatureValidationMethod demandSignature)
        {
            if (metadataUrl == null)
            {
                throw new ArgumentNullException("metadataUrl");
            }
            return (ExtendedEntitiesDescriptor)Load(metadataUrl, signingCertificate, demandSignature);
        }
    }
}
