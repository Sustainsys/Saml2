using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sustainsys.Saml2.Internal
{
    public static class TokenValidationParametersExtensions
    {
        private static readonly PropertyInfo requireAudienceProperty = typeof(TokenValidationParameters).GetProperty("RequireAudience");

        public static TokenValidationParameters SetRequireAudience(this TokenValidationParameters tokenValidationParameters, bool value)
        {
            if (requireAudienceProperty != null)
            {
                requireAudienceProperty.SetValue(tokenValidationParameters, value);
            }

            return tokenValidationParameters;
        }
    }
}
