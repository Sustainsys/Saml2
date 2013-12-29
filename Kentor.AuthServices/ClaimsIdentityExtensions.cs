using System;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Linq;

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
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            var assertion = new Saml2Assertion(new Saml2NameIdentifier(issuer));

            assertion.Subject = new Saml2Subject(new Saml2NameIdentifier(
                identity.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value));

            assertion.Conditions = new Saml2Conditions()
            {
                NotOnOrAfter = DateTime.UtcNow.AddMinutes(2)
            };

            return assertion;
        }
    }
}
