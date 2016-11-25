using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Kentor.AuthServices.Internal
{
    internal static class CryptographyExtensions
    {
        internal static void Encrypt(this XmlElement elementToEncrypt, bool useOaep, X509Certificate2 certificate)
        {
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));

            var encryptedData = new EncryptedData
            {
                Type = EncryptedXml.XmlEncElementUrl,
                EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url)
            };

            var algorithm = useOaep ? EncryptedXml.XmlEncRSAOAEPUrl : EncryptedXml.XmlEncRSA15Url;
            var encryptedKey = new EncryptedKey
            {
                EncryptionMethod = new EncryptionMethod(algorithm),
            };

            var encryptedXml = new EncryptedXml();
            byte[] encryptedElement;
            using (var symmetricAlgorithm = new RijndaelManaged())
            {
                symmetricAlgorithm.KeySize = 256;
                encryptedKey.CipherData = new CipherData(EncryptedXml.EncryptKey(symmetricAlgorithm.Key, (RSA)certificate.PublicKey.Key, useOaep));
                encryptedElement = encryptedXml.EncryptData(elementToEncrypt, symmetricAlgorithm, false);
            }
            encryptedData.CipherData.CipherValue = encryptedElement;

            encryptedData.KeyInfo = new KeyInfo();
            encryptedData.KeyInfo.AddClause(new KeyInfoEncryptedKey(encryptedKey));
            EncryptedXml.ReplaceElement(elementToEncrypt, encryptedData, false);
        }

        internal static IEnumerable<XmlElement> Decrypt(this IEnumerable<XmlElement> elements, AsymmetricAlgorithm key)
        {
            foreach (var element in elements)
            {
                yield return element.Decrypt(key);
            }
        }

        internal static XmlElement Decrypt(this XmlElement element, AsymmetricAlgorithm key)
        {
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml(element.OuterXml);

            var exml = new RSAEncryptedXml(xmlDoc, (RSA)key);

            exml.DecryptDocument();

            return xmlDoc.DocumentElement;
        }
    }
}
