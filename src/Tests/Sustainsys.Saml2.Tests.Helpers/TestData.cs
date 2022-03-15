using Sustainsys.Saml2.Metadata.Xml;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Helpers
{
    public static class TestData
    {
        public static XmlTraverser GetXmlTraverser<TDirectory>([CallerMemberName] string? testName = null)
        {
            ArgumentNullException.ThrowIfNull(testName);

            var fileName = "..\\..\\..\\" + typeof(TDirectory).Name + "\\" + testName + ".xml";

            var document = new XmlDocument();
            document.Load(fileName);

            return new XmlTraverser(document.DocumentElement ?? throw new InvalidOperationException("XmlDoc contained no DocumentElement"));
        }
    }
}