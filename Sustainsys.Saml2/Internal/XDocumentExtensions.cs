using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Sustainsys.Saml2.Internal
{
    static class XDocumentExtensions
    {
        public static string ToStringWithXmlDeclaration(this XDocument xDocument)
        {
            if (xDocument.Declaration != null)
            {
                return xDocument.Declaration?.ToString() + "\r\n" + xDocument.ToString();
            }

            return xDocument.ToString();
        }
    }
}
