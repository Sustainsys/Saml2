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
                ?? throw new Saml2MetadataException($"Required attribute {localName} not found on {xmlReader.Name}");
        }
    }
}
