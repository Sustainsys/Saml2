using System;
using System.Collections;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using Kentor.AuthServices.Configuration;

namespace Kentor.AuthServices
{
    class Saml2EncryptedAssertion
    {
        /// <summary>
        /// Interate through the <see cref="KeyInfo"/> looking for the first <see cref="KeyInfoEncryptedKey" /> type.
        /// </summary>
        /// <param name="keyInfo">The enumeration to process.</param>
        /// <returns>The first <see cref="KeyInfoEncryptedKey" /> found in the collecion.</returns>
        private static T GetKeyInfo<T>(KeyInfo keyInfo)
        {
            if (keyInfo == null) 
            {
                throw new ArgumentNullException("keyInfo", "The encrypted keyInfo cannot be null");
            }

            return keyInfo.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the encryption algorithm to use for the specified encrypted key info.
        /// </summary>
        /// <param name="algorithm">The Xmldsig namespace for the algorithm.</param>
        /// <returns>The <see cref="SymmetricAlgorithm"/> to use to decrypt the data.</returns>
        private static SymmetricAlgorithm GetAlgorithm(string algorithm)
        {
            switch (algorithm) 
            {
                case "http://www.w3.org/2001/04/xmlenc#tripledes-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-tripledes": 
                    {
                        return new TripleDESCryptoServiceProvider();
                    }

                case "http://www.w3.org/2001/04/xmlenc#aes128-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes192-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes128":
                case "http://www.w3.org/2001/04/xmlenc#aes256-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes192":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes256": 
                    {
                        return new RijndaelManaged();                    
                    }
            }
            
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, 
                                                              "An unknown encryption algorithm '{0}' was specified. Cannot decrypt the response.",
                                                              algorithm));
        }

        /// <summary>
        /// Find the local certificate with private key that matches the encrypted key info. 
        /// </summary>
        /// <param name="keyInfo">The key info to search for.</param>
        /// <returns>
        /// The matching x509 certificate from the local store that has a private key. 
        /// If the certificate is not found, then an <see cref="CertificateNotFoundException"/> is thrown.
        /// If the certificate is found, but a private key is not available, then an 
        /// <see cref="CertificatePrivateKeyNotFoundException" /> is thrown.
        /// </returns>
        private static X509Certificate2 GetCertificateWithPrivateKey(KeyInfoEncryptedKey keyInfo)
        {
            if (keyInfo == null) 
            {
                throw new ArgumentNullException("keyInfo", "The encrypted keyInfo cannot be null");
            }

            KeyInfoX509Data x509DataKeyInfo = GetKeyInfo<KeyInfoX509Data>(keyInfo.EncryptedKey.KeyInfo);
            X509Certificate2 keyCertificate = (X509Certificate2) x509DataKeyInfo.Certificates[0];
            var thumbprint = keyCertificate.Thumbprint;
            X509Certificate2 certificate = CertificateUtilities.GetCertificate(new[] { StoreName.My },
                                                                               new[] { StoreLocation.LocalMachine, StoreLocation.CurrentUser },
                                                                               X509FindType.FindByThumbprint,
                                                                               thumbprint);
            if (! certificate.HasPrivateKey) 
            {
                throw new CertificatePrivateKeyNotFoundException(certificate);
            }
            return (certificate);
        }

        /// <summary>
        /// Checks to see if the specified document has an encrpted assertion.
        /// </summary>
        /// <param name="document">The document to inspect.</param>
        /// <returns></returns>
        public static bool IsEncrypted(XmlDocument document)
        {
            if ((document == null) || (document.DocumentElement == null)) 
            {
                throw new ArgumentNullException("document", "The xml document was null.");
            }

            bool isEncrypted = document.DocumentElement.ChildNodes.Cast<XmlNode>()
                .Where(node => node.NodeType == XmlNodeType.Element && node.NamespaceURI == Saml2Namespaces.Saml2Name).Cast<XmlElement>()
                .Any(xe => xe.LocalName == "EncryptedAssertion");
            return isEncrypted;
        }

        /// <summary>
        /// Decrypts the EncryptedAssertion contained within the XmlElement.
        /// </summary>
        /// <param name="element">The EncryptedAssertion element to decrypt.</param>
        /// <returns>The decrypted element.</returns>
        public static XmlElement Decrypt(XmlElement element)
        {
            if (element == null) 
            {
                return (null);
            }
            if (element.LocalName != "EncryptedAssertion")
            {
                return (element);
            }

            XmlElement data = element.ChildNodes.Cast<XmlElement>()
                .FirstOrDefault(xe => xe.LocalName == "EncryptedData");

            var encryptedData = new EncryptedData();
            encryptedData.LoadXml(data);

            KeyInfoEncryptedKey encryptedKeyInfo = GetKeyInfo<KeyInfoEncryptedKey>(encryptedData.KeyInfo);
            X509Certificate2 certificate = GetCertificateWithPrivateKey(encryptedKeyInfo);

            SymmetricAlgorithm algorithm = GetAlgorithm(encryptedData.EncryptionMethod.KeyAlgorithm);
            algorithm.Key = (certificate.PrivateKey as RSACryptoServiceProvider).Decrypt(encryptedKeyInfo.EncryptedKey.CipherData.CipherValue, true);
            algorithm.Padding = PaddingMode.ISO10126;
            algorithm.Mode = CipherMode.CBC;

            int ivSize = algorithm.BlockSize / 8;
            byte[] iv = new byte[ivSize];
            Buffer.BlockCopy(encryptedData.CipherData.CipherValue, 0, iv, 0, iv.Length);
            using (ICryptoTransform decrTransform = algorithm.CreateDecryptor(algorithm.Key, iv)) 
            {
                byte[] plainText = decrTransform.TransformFinalBlock(encryptedData.CipherData.CipherValue, iv.Length,
                                                                     encryptedData.CipherData.CipherValue.Length - iv.Length);

                XmlDocument assertion = new XmlDocument();
                assertion.LoadXml(UTF8Encoding.UTF8.GetString(plainText));
                return (assertion.DocumentElement);
            }
        }
    }
}
