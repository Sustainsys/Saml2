using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Kentor.AuthServices.Tests
{
    class SignedXmlHelper
    {
        public static readonly X509Certificate2 TestCert = new X509Certificate2("Kentor.AuthServices.Tests.pfx");
        
        public static string SignXml(string xml)
        {
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml(xml);

            xmlDoc.Sign(TestCert);

            return xmlDoc.OuterXml;
        }
    }
}
