using Microsoft.IdentityModel.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Metadata
{
    internal class FilteringXmlDictionaryReader : DelegatingXmlDictionaryReader
    {
        private string xmlNamespaceUri, name;

        public FilteringXmlDictionaryReader(string xmlNamespaceUri, string name, XmlDictionaryReader innerReader)
        {
            this.xmlNamespaceUri = xmlNamespaceUri;
            this.name = name;

            InnerReader = innerReader;
        }

        public override bool Read()
        {
            bool result;
            do
            {
                result = InnerReader.Read();
            }
            while (result
                && InnerReader.NamespaceURI == xmlNamespaceUri
                && InnerReader.LocalName == name);

            return result;
        }

        // Sigh.  ASPNET core bug.
        // XmlReader:
        // public void Dispose()
        // {
        //     Dispose(true);
        // }
        // protected virtual void Dispose(bool disposing)
        // {
        //     if (disposing && ReadState != ReadState.Closed)
        //     {
        //         Close();
        //     }
        // }
        //
        // XmlDictionaryReader:
        // public override void Close()
        // {
        // 	base.Dispose();
        // }
        // = stack overflow
        //
        public override void Close()
        {
        }
    }
}