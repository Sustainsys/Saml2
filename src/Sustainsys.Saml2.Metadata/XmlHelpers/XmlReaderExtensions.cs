using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sustainsys.Saml2.Metadata.XmlHelpers
{
    public static class XmlReaderExtensions
    {
        public static string GetRequiredAttribute(this XmlReader xmlReader, string localName)
        {
            return xmlReader.GetAttribute(localName)
                ?? throw new Saml2XmlException($"Required attribute {localName} not found on {xmlReader.Name}");
        }

        public static void EnsureName(this XmlReader xmlReader, string namespaceUri, string localName)
        {
            if (xmlReader.Name != localName)
            {
                throw new Saml2XmlException($"Unexpected node name \"{xmlReader.LocalName}\", expected \"{localName}\"");
            }

            if (xmlReader.NamespaceURI != namespaceUri)
            {
                throw new Saml2XmlException($"Unexpected namespace \"{xmlReader.NamespaceURI}\" for local name \"{xmlReader.Name}\", expected \"{namespaceUri}\"");
            }
        }
    }
}
