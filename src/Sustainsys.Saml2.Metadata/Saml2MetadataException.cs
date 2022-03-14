using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Metadata
{
    public class Saml2MetadataException : Exception
    {
        public Saml2MetadataException(string message)
            : base(message) { }
    }
}
