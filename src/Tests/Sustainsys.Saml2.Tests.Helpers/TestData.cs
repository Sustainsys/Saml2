using System.Runtime.CompilerServices;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Helpers
{
    public static class TestData
    {
        public static XmlReader GetXmlReader<TDirectory>([CallerMemberName] string? testName = null)
        {
            ArgumentNullException.ThrowIfNull(testName);

            var fileName = "..\\..\\..\\" + typeof(TDirectory).Name + "\\" + testName + ".xml";

            var document = new XmlDocument();
            document.Load(fileName);

            var reader = new XmlNodeReader(document);

            reader.MoveToContent();

            return reader;
        }
    }
}