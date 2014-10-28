using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kentor.AuthServices.Metadata
{
    class XmlFilteringReader : DelegatingXmlDictionaryReader
    {
        string xmlNamespaceUri, name;

        public XmlFilteringReader(string xmlNamespaceUri, string name, XmlDictionaryReader innerReader)
        {
            this.xmlNamespaceUri = xmlNamespaceUri;
            this.name = name;

            InitializeInnerReader(innerReader);
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
    }
}
