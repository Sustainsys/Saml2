using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
#if NET45
using System.IdentityModel.Metadata;
#endif
using System.Text;

namespace Kentor.AuthServices.Metadata
{
    public static class Saml2NameIdentifierExtensions
    {
        public static EntityId AsEntityId(this Saml2NameIdentifier nameIdentifier)
        {
            return new EntityId(nameIdentifier.Value);
        }
    }
}
