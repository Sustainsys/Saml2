using System;
using System.IdentityModel.Tokens;
using System.Security.Claims;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Extension methods for claims.
    /// </summary>
    public static class ClaimsExtensions
    {
        /// <summary>
        /// Create a Saml2NameIdentifier from a NameIdentifier claim.
        /// </summary>
        /// <param name="nameIdClaim">Name identifier claim.</param>
        /// <returns>Saml2NameIdentifier</returns>
        public static Saml2NameIdentifier ToSaml2NameIdentifier(this Claim nameIdClaim)
        {
            if(nameIdClaim == null)
            {
                throw new ArgumentNullException(nameof(nameIdClaim));
            }

            var saml2NameIdentifier = new Saml2NameIdentifier(nameIdClaim.Value);

            nameIdClaim.ExtractProperty(ClaimProperties.SamlNameIdentifierFormat,
                value => saml2NameIdentifier.Format = new Uri(value));
            nameIdClaim.ExtractProperty(ClaimProperties.SamlNameIdentifierNameQualifier,
                value => saml2NameIdentifier.NameQualifier = value);
            nameIdClaim.ExtractProperty(ClaimProperties.SamlNameIdentifierSPNameQualifier,
                value => saml2NameIdentifier.SPNameQualifier = value);
            nameIdClaim.ExtractProperty(ClaimProperties.SamlNameIdentifierSPProvidedId,
                value => saml2NameIdentifier.SPProvidedId = value);

            return saml2NameIdentifier;
        }

        private static void ExtractProperty(
            this Claim claim,
            string propertyKey,
            Action<string> propertySetter)
        {
            string value;
            if(claim.Properties.TryGetValue(propertyKey, out value))
            {
                propertySetter(value);
            }
        }
    }
}
