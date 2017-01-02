using Kentor.AuthServices.Internal;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Certificates used by the service provider for signing or decryption.
    /// </summary>
    public class ServiceCertificateCollection: Collection<ServiceCertificate>
    {
        /// <summary>
        /// Add a certificate to the collection with default status use and
        /// metadata behaviour.
        /// </summary>
        /// <param name="certificate">Certificate to add.</param>
        public void Add(X509Certificate2 certificate)
        {
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));
            InsertItem(base.Count, new ServiceCertificate
            {
                Certificate = certificate
            });
        }

        /// <summary>
        /// Add to the collection at the specified position.
        /// </summary>
        /// <param name="index">Position index.</param>
        /// <param name="item">Service certificate to add.</param>
        protected override void InsertItem(int index, ServiceCertificate item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (!item.Certificate.HasPrivateKey)
            {
                throw new ArgumentException(@"Provided certificate is not valid because it does not contain a private key.");
            }

            if (item.Use == CertificateUse.Encryption || item.Use == CertificateUse.Both)
            {
                if (!CertificateWorksForDecryption(item.Certificate))
                {
                    throw new ArgumentException(@"Provided certificate is not valid for encryption/decryption. There may be insufficient permissions to its private key in the windows certificate store or the certificate itself may not have the correct purposes. If you only want to use it for signing, set the Use property to Signing (CertificateUse.Signing).");
                }
            }
            base.InsertItem(index, item);
        }

        private static bool CertificateWorksForDecryption(X509Certificate2 certificate)
        {
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml("<xml/>");
            var elementToEncrypt = xmlDoc.DocumentElement;
            elementToEncrypt.Encrypt(useOaep: false, certificate: certificate);

            try
            {
                elementToEncrypt.OwnerDocument.DocumentElement.Decrypt(certificate.PrivateKey);
            }
            catch (CryptographicException)
            {
                return false;
            }
            return true;
        }
    }
}
