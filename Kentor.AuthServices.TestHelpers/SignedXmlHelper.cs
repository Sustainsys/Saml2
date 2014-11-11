using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;

namespace Kentor.AuthServices.TestHelpers
{
    public class SignedXmlHelper
    {
        public static readonly X509Certificate2 TestCert = new X509Certificate2("Kentor.AuthServices.Tests.pfx");

        public static readonly X509Certificate2 TestCert2 = new X509Certificate2("Kentor.AuthServices.Tests2.pfx");

        public static readonly AsymmetricAlgorithm TestKey = TestCert.PublicKey.Key;

        public static readonly KeyDescriptor TestKeyDescriptor = new KeyDescriptor(
            new SecurityKeyIdentifier(
                (new X509SecurityToken(TestCert))
                .CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>()));

        public static string SignXml(string xml, bool includeKeyInfo = false, bool preserveWhitespace = true)
        {
            var xmlDoc = new XmlDocument { PreserveWhitespace = preserveWhitespace };
            xmlDoc.LoadXml(xml);

            xmlDoc.Sign(TestCert, includeKeyInfo);

            return xmlDoc.OuterXml;
        }        

        public static string EncryptXml(string xml, string elementToEncryptName)
        {
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml(xml);

            // Create a new TripleDES key. 
            var sessionKey = new RijndaelManaged { KeySize = 256 };

            var encryptedData = new EncryptedData
            {
                Type = EncryptedXml.XmlEncElementUrl,
                EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url)
            };

            var elementToEncrypt = (XmlElement) xmlDoc.GetElementsByTagName(elementToEncryptName, Saml2Namespaces.Saml2Name)[0];

            // Encrypt the assertion and add it to the encryptedData instance.
            var encryptedXml = new EncryptedXml();
            var encryptedElement = encryptedXml.EncryptData(elementToEncrypt, sessionKey, false);
            encryptedData.CipherData.CipherValue = encryptedElement;

            // Add an encrypted version of the key used.
            encryptedData.KeyInfo = new KeyInfo();

            var encryptedKey = new EncryptedKey
            {
                EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncRSA15Url),
                CipherData = new CipherData(EncryptedXml.EncryptKey(sessionKey.Key, (RSA)TestCert2.PublicKey.Key, false))
            };

            encryptedData.KeyInfo.AddClause(new KeyInfoEncryptedKey(encryptedKey));

            EncryptedXml.ReplaceElement(elementToEncrypt, encryptedData, false);

            return xmlDoc.OuterXml;
        }

        public static readonly string KeyInfoXml;

        static SignedXmlHelper()
        {
            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(TestCert));

            KeyInfoXml = keyInfo.GetXml().OuterXml;
        }

    }
}
