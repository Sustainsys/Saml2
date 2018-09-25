using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Internal;
using Sustainsys.Saml2.Tokens;
using System;
using System.Linq;
using System.Security.Claims;

namespace Sustainsys.Saml2.Saml2P
{
	/// <summary>
	/// Somewhat ugly subclassing to be able to access some methods that are protected
	/// on Saml2SecurityTokenHandler. The public interface of Saml2SecurityTokenHandler
	/// expects the actual assertion to be signed, which is not always the case when
	/// using Saml2-P. The assertion can be embedded in a signed response. Or the signing
	/// could be handled at transport level.
	/// </summary>
	public class Saml2PSecurityTokenHandler : Saml2SecurityTokenHandler
    {
		public Saml2PSecurityTokenHandler()
		{
			Serializer = new Saml2PSerializer();
		}

		// Overridden to fix the fact that the base class version uses NotBefore as the token replay expiry time
		// Due to the fact that we can't override the ValidateToken function (it's overridden in the base class!)
		// we have to parse the token again.
		// This can be removed when:
		// https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/898
		// is fixed.
		protected override void ValidateTokenReplay(DateTime? expirationTime, string securityToken, TokenValidationParameters validationParameters)
		{
			var saml2Token = ReadSaml2Token(securityToken);
			base.ValidateTokenReplay(saml2Token.Assertion.Conditions.NotOnOrAfter,
				securityToken, validationParameters);
		}

		// TODO: needed with Microsoft.identitymodel?
		/// <summary>
		/// Process authentication statement from SAML assertion. WIF chokes if the authentication statement 
		/// contains a DeclarationReference, so we clear this out before calling the base method
		/// http://referencesource.microsoft.com/#System.IdentityModel/System/IdentityModel/Tokens/Saml2SecurityTokenHandler.cs,1970
		/// </summary>
		/// <param name="statement">Authentication statement</param>
		/// <param name="subject">Claim subject</param>
		/// <param name="issuer">Assertion Issuer</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected override void ProcessAuthenticationStatement(Saml2AuthenticationStatement statement, ClaimsIdentity subject, string issuer)
        {
            if (statement.AuthenticationContext != null)
            {
                statement.AuthenticationContext.DeclarationReference = null;
            }
            base.ProcessAuthenticationStatement(statement, subject, issuer);

            if(statement.SessionIndex != null)
            {
                var nameIdClaim = subject.FindFirst(ClaimTypes.NameIdentifier);

                if (nameIdClaim != null)
                {
                    subject.AddClaim(
                        new Claim(
                            Saml2ClaimTypes.LogoutNameIdentifier,
                            DelimitedString.Join(
                                nameIdClaim.Properties.GetValueOrEmpty(ClaimProperties.SamlNameIdentifierNameQualifier),
                                nameIdClaim.Properties.GetValueOrEmpty(ClaimProperties.SamlNameIdentifierSPNameQualifier),
                                nameIdClaim.Properties.GetValueOrEmpty(ClaimProperties.SamlNameIdentifierFormat),
                                nameIdClaim.Properties.GetValueOrEmpty(ClaimProperties.SamlNameIdentifierSPProvidedId),
                                nameIdClaim.Value),
                            null,
                            issuer));
                }

                subject.AddClaim(
                    new Claim(Saml2ClaimTypes.SessionIndex, statement.SessionIndex, null, issuer));
            }
        }

		protected override Saml2SecurityToken ValidateSignature(string token, TokenValidationParameters validationParameters)
		{
			// Just skip signature validation -- we do this elsewhere
			return ReadSaml2Token(token);
		}
    }
}
