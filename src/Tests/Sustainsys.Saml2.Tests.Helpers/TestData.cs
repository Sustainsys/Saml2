using Sustainsys.Saml2.Metadata.Xml;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Helpers
{
    public static class TestData
    {
        public static XmlTraverser GetXmlTraverser<TDirectory>([CallerMemberName] string? testName = null)
        {
            ArgumentNullException.ThrowIfNull(testName);

            var fileName = "../../../" + typeof(TDirectory).Name + "/" + testName + ".xml";

            var document = new XmlDocument();
            document.Load(fileName);

            return new XmlTraverser(document.DocumentElement ?? throw new InvalidOperationException("XmlDoc contained no DocumentElement"));
        }

        public static X509Certificate2 Certificate = new X509Certificate2("Sustainsys.Saml2.Tests.pfx");

        public static SigningKey SigningKey = new()
        {
            Certificate = Certificate,
            TrustLevel = TrustLevel.ConfiguredKey
        };

        public static SigningKey[] SingleSigningKey =
        {
            SigningKey
        };

        public static SigningKey SigningKey2 = new()
        {
            Certificate = new X509Certificate2("Sustainsys.Saml2.Tests2.pfx"),
            TrustLevel = TrustLevel.TLS
        };

        public static SigningKey[] SingleSigningKey2 =
        {
            SigningKey2
        };
    }
}