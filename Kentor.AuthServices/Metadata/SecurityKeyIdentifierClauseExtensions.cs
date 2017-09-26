using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kentor.AuthServices.Metadata
{
    public static class SecurityKeyIdentifierClauseExtensions
    {
        public static SecurityKey CreateKey(this SecurityKeyIdentifierClause clause)
            => ((X509RawDataKeyIdentifierClause)clause).CreateKey();
    }
}
