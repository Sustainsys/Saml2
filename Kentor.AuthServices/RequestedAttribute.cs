using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class RequestedAttribute : Saml2Attribute
    {
        public RequestedAttribute(string name)
            : base(name)
        {

        }
    }
}
