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

        public static readonly AsymmetricAlgorithm TestKey2 = TestCert2.PublicKey.Key;

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

        public static string EncryptAssertion(string assertionXml, bool useOaep = false)
        {            
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            var wrappedAssertion = string.Format(@"<saml2:EncryptedAssertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion"">{0}</saml2:EncryptedAssertion>", assertionXml);
            xmlDoc.LoadXml(wrappedAssertion);

            var symmetricAlgorithm = new RijndaelManaged { KeySize = 256 };

            var encryptedData = new EncryptedData
            {
                Type = EncryptedXml.XmlEncElementUrl,
                EncryptionMethod = new System.Security.Cryptography.Xml.EncryptionMethod(EncryptedXml.XmlEncAES256Url)
            };

            var elementToEncrypt = (XmlElement) xmlDoc.GetElementsByTagName("Assertion", Saml2Namespaces.Saml2Name)[0];

            // Encrypt the assertion and add it to the encryptedData instance.
            var encryptedXml = new EncryptedXml();
            var encryptedElement = encryptedXml.EncryptData(elementToEncrypt, symmetricAlgorithm, false);
            encryptedData.CipherData.CipherValue = encryptedElement;

            // Add an encrypted version of the key used.
            encryptedData.KeyInfo = new KeyInfo();

            var algorithm = useOaep ? EncryptedXml.XmlEncRSAOAEPUrl : EncryptedXml.XmlEncRSA15Url;
            var encryptedKey = new EncryptedKey
            {
                EncryptionMethod = new System.Security.Cryptography.Xml.EncryptionMethod(algorithm),
                CipherData = new CipherData(EncryptedXml.EncryptKey(symmetricAlgorithm.Key, (RSA)TestCert2.PublicKey.Key, useOaep))
            };

            encryptedData.KeyInfo.AddClause(new KeyInfoEncryptedKey(encryptedKey));

            EncryptedXml.ReplaceElement(elementToEncrypt, encryptedData, false);

            return xmlDoc.OuterXml;
        }

        public static readonly string KeyInfoXml;

        public static readonly string KeyInfoXml2;

        static SignedXmlHelper()
        {
            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(TestCert));
            KeyInfoXml = keyInfo.GetXml().OuterXml;

            var keyInfo2 = new KeyInfo();
            keyInfo2.AddClause(new KeyInfoX509Data(TestCert2));
            KeyInfoXml2 = keyInfo2.GetXml().OuterXml;
        }
    }
}
