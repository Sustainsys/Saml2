using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using Kentor.AuthServices.Internal;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace Kentor.AuthServices.Tests.Helpers
{
    public class SignedXmlHelper
    {
        public static readonly X509Certificate2 TestCert = new X509Certificate2("Kentor.AuthServices.Tests.pfx");

        public static readonly X509Certificate2 TestCert2 = new X509Certificate2("Kentor.AuthServices.Tests2.pfx");

        public static readonly X509Certificate2 TestCertSignOnly = new X509Certificate2("Kentor.AuthServices.TestsSignOnly.pfx");

        public static readonly RsaKeyIdentifierClause TestKey =
            new RsaKeyIdentifierClause((RSA)TestCert.PublicKey.Key);

        public static readonly RsaKeyIdentifierClause TestKey2 =
            new RsaKeyIdentifierClause((RSA)TestCert2.PublicKey.Key);

        public static readonly RsaKeyIdentifierClause TestKeySignOnly =
            new RsaKeyIdentifierClause((RSA)TestCertSignOnly.PublicKey.Key);

        public static readonly KeyDescriptor TestKeyDescriptor = new KeyDescriptor(
            new SecurityKeyIdentifier(
                (new X509SecurityToken(TestCertSignOnly))
                .CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>()));

        public static string SignXml(string xml, bool includeKeyInfo = false, bool preserveWhitespace = true)
        {
            var xmlDoc = new XmlDocument { PreserveWhitespace = preserveWhitespace };
            xmlDoc.LoadXml(xml);

            xmlDoc.Sign(TestCert, includeKeyInfo);

            return xmlDoc.OuterXml;
        }        

        public static string EncryptAssertion(string assertionXml, bool useOaep = false, X509Certificate2 certificate = null)
        {
            if (certificate == null)
            {
                certificate = TestCert2;
            }

            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            var wrappedAssertion = $@"<saml2:EncryptedAssertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion"">{assertionXml}</saml2:EncryptedAssertion>";
            xmlDoc.LoadXml(wrappedAssertion);
            var elementToEncrypt = (XmlElement)xmlDoc.GetElementsByTagName("Assertion", Saml2Namespaces.Saml2Name)[0];

            elementToEncrypt.Encrypt( useOaep, certificate);

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

        public static void RemoveGlobalSha256XmlSignatureSupport()
        {
            // Clean up after tests that globally activate SHA256 support. There
            // is no official API for removing signature algorithms, so let's
            // do some reflection.

            var internalSyncObject = typeof(CryptoConfig)
                .GetProperty("InternalSyncObject", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null);

            lock (internalSyncObject)
            {
                var appNameHT = (IDictionary<string, Type>)typeof(CryptoConfig)
                    .GetField("appNameHT", BindingFlags.Static | BindingFlags.NonPublic)
                    .GetValue(null);

                appNameHT.Remove("http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
            }
        }
    }
}
