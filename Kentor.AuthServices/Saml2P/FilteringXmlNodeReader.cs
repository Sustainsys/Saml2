using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kentor.AuthServices.Saml2P
{
    class FilteringXmlNodeReader : XmlNodeReader
    {
        string filterNamespace;
        string filterNode;

        public FilteringXmlNodeReader(string filterNamespace, string filterNode, XmlNode source)
            : base(source)
        {
            this.filterNamespace = filterNamespace;
            this.filterNode = filterNode;
        }

        public override bool Read()
        {
            var result = base.Read();

            if(result
                && LocalName == filterNode
                && NamespaceURI == filterNamespace)
            {
                Skip();
                // Skip calls read assume that the result was true.
                result = true;
            }

            return result;
        }
    }
}
