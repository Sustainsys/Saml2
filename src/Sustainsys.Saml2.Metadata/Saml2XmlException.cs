using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Metadata
{
    public class Saml2XmlException : Exception
    {
        public Saml2XmlException(string message)
            : base(message) { }
    }
}
