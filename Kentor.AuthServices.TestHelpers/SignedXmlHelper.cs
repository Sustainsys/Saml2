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

        public static readonly string KeyInfoXml;

        static SignedXmlHelper()
        {
            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(TestCert));

            KeyInfoXml = keyInfo.GetXml().OuterXml;
        }

    }
}
