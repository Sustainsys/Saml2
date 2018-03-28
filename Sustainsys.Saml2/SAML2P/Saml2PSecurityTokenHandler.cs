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
        private SPOptions spOptions;
		public SecurityTokenHandlerConfiguration Configuration { get; private set; }

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="spOptions">Options for the service provider that
		/// this token handler should work with.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public Saml2PSecurityTokenHandler(SPOptions spOptions)
        {
            if (spOptions == null)
            {
                throw new ArgumentNullException(nameof(spOptions));
            }

	        Configuration = new SecurityTokenHandlerConfiguration
            {
                IssuerNameRegistry = new ReturnRequestedIssuerNameRegistry(),
                AudienceRestriction = GetAudienceRestriction(spOptions),
                SaveBootstrapContext = spOptions.SystemIdentityModelIdentityConfiguration.SaveBootstrapContext
            };
	        this.spOptions = spOptions;

			Serializer = new Saml2PSerializer(spOptions);
        }

		protected override Saml2Conditions CreateConditions(SecurityTokenDescriptor tokenDescriptor)
		{
			var conditions = base.CreateConditions(tokenDescriptor);
			conditions.AudienceRestrictions.Clear();
			conditions.AudienceRestrictions.Add(
				new Saml2AudienceRestriction(Configuration.AudienceRestriction
					.AllowedAudienceUris.Select(x => x.ToString())));
			return conditions;
		}

		/// <summary>
		/// Create claims from the token.
		/// </summary>
		/// <param name="samlToken">The token to translate to claims.</param>
		/// <returns>An identity with the created claims.</returns>
		protected override ClaimsIdentity CreateClaimsIdentity(Saml2SecurityToken samlToken, string issuer, TokenValidationParameters validationParameters)
		// ?? public new ClaimsIdentity CreateClaims(Saml2SecurityToken samlToken)
        {
            var identity = base.CreateClaimsIdentity(samlToken, issuer, validationParameters);

            if (spOptions.SystemIdentityModelIdentityConfiguration.SaveBootstrapContext)
            {
                // TODO: identity.BootstrapContext = new BootstrapContext(samlToken, this);
            }

            return identity;
        }

#if TODO
        /// <summary>
        /// Detect if a token is replayed (i.e. reused). The token is added to the
        /// list of used tokens, so this method should only be called once for each token.
        /// </summary>
        /// <param name="token">The token to check.</param>
        public new void DetectReplayedToken(SecurityToken token)
        {
            base.DetectReplayedToken(token);
        }

        /// <summary>
        /// Validate the conditions of the token.
        /// </summary>
        /// <param name="conditions">Conditions to check</param>
        /// <param name="enforceAudienceRestriction">Should the audience restriction be enforced?</param>
        public new void ValidateConditions(Saml2Conditions conditions, bool enforceAudienceRestriction)
        {
            base.ValidateConditions(conditions, enforceAudienceRestriction);
        }
#endif

#if TODO
		public override SecurityToken ReadToken(XmlReader reader, TokenValidationParameters validationParameters)
		{
			var token = new Saml2SecurityToken(Serializer.ReadAssertion(reader));
			// TODO: I don't think token text matters here?
			var samlToken = ValidateSignature(token.ToString(), validationParameters);
			ValidateConditions(token, validationParameters);
			ValidateSubject(token, validationParameters);
			var issuer = ValidateIssuer(token.Issuer, token, validationParameters);
			// TODO: check token again.  I don't think this check is implemented
			ValidateTokenReplay(token.Assertion.Conditions.NotBefore, token.ToString(), validationParameters);
			ValidateIssuerSecurityKey(token.SigningKey, token, validationParameters);
			//validatedToken = samlToken;
			//var identity = CreateClaimsIdentity(samlToken, issuer, validationParameters);
			//if (validationParameters.SaveSigninToken)
				//identity.BootstrapContext = token;

			return token;
		}

		public ClaimsIdentity CreateClaimsIdentity2(Saml2SecurityToken samlToken, string issuer, TokenValidationParameters validationParameters)
		{
			return base.CreateClaimsIdentity(samlToken, issuer, validationParameters);
		}
#endif

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

        /// <summary>
        /// Check if an audience restriction from configuration should be
        /// applied or if we should revert to the default behaviour of
        /// restricting the audience to the entity id.
        /// </summary>
        /// <param name="spOptions">Sp Options with configuration</param>
        /// <returns>Configured or created audience restriction.</returns>
        private static AudienceRestriction GetAudienceRestriction(SPOptions spOptions)
        {
            var audienceRestriction = spOptions.SystemIdentityModelIdentityConfiguration.AudienceRestriction;

            if (audienceRestriction.AudienceMode != AudienceUriMode.Never
                && !audienceRestriction.AllowedAudienceUris.Any())
            {
                // Create a new instance instead of modifying the one from the
                // configuration.
                audienceRestriction = new AudienceRestriction(audienceRestriction.AudienceMode);
                audienceRestriction.AllowedAudienceUris.Add(new Uri(spOptions.EntityId.Id));
            }

            return audienceRestriction;
        }
    }
}
