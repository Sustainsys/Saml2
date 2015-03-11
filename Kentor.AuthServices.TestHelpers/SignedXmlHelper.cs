using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Kentor.AuthServices;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;

namespace Kentor.AuthServices.TestHelpers
{
    public class SignedXmlHelper
    {
        public static readonly X509Certificate2 TestCert = new X509Certificate2("Kentor.AuthServices.Tests.pfx");
        public static readonly X509SigningCredentials SigningCredentitals
            = new X509SigningCredentials(TestCert, 
                SecurityAlgorithms.RsaSha1Signature, SecurityAlgorithms.Sha1Digest);

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

        public static string SignAssertion(Saml2Assertion assertion, bool includeKeyInfo = true)
        {
            string signedAssertion = String.Empty;
            var token = new Saml2SecurityToken(assertion);

            var handler = new Saml2SecurityTokenHandler();
            assertion.SigningCredentials = SigningCredentitals;

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter,
                    new XmlWriterSettings { OmitXmlDeclaration = true }))
                {
                    handler.WriteToken(xmlWriter, token);
                }
                signedAssertion = stringWriter.ToString();
                if (!includeKeyInfo)
                {
                    // http://stackoverflow.com/questions/28995480/create-saml2-assertion-without-keyinfo
                    signedAssertion = Regex.Replace(signedAssertion, @"(<KeyInfo.*?</KeyInfo>\s*)+", "");
                }
            }
            return signedAssertion;
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
