using System;
using System.IdentityModel.Tokens;
using System.Security.Claims;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Extension methods for Claims Identities
    /// </summary>
    public static class ClaimsIdentityExtensions
    {
        /// <summary>
        /// Creates a Saml2Assertion from a ClaimsIdentity.
        /// </summary>
        /// <returns>Saml2Assertion</returns>
        public static Saml2Assertion ToSaml2Assertion(this ClaimsIdentity identity, string issuer)
        {
            throw new NotImplementedException();
        }
    }
}
