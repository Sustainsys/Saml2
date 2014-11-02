using Kentor.AuthServices.Internal;
using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Security.Claims;

namespace Kentor.AuthServices.Saml2P
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
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="spEntityId">The entity id of this service provider, which is used
        /// to validate the audience restrictions of the incoming assertions.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public Saml2PSecurityTokenHandler(EntityId spEntityId)
        {
            if(spEntityId == null)
            {
                throw new ArgumentNullException("spEntityId");
            }

            FederatedAuthentication.FederationConfiguration.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris.Add(
                new Uri(spEntityId.Id));

            if (FederatedAuthentication.FederationConfiguration.IdentityConfiguration.IssuerNameRegistry is ConfigurationBasedIssuerNameRegistry
                &&
                (FederatedAuthentication.FederationConfiguration.IdentityConfiguration.IssuerNameRegistry as
                    ConfigurationBasedIssuerNameRegistry).ConfiguredTrustedIssuers.Count == 0)
            {
                FederatedAuthentication.FederationConfiguration.IdentityConfiguration.IssuerNameRegistry =
                    new ReturnRequestedIssuerNameRegistry();
            }

            Configuration = new SecurityTokenHandlerConfiguration
            {
                IssuerNameRegistry = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.IssuerNameRegistry,
                AudienceRestriction = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.AudienceRestriction,
                SaveBootstrapContext = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SaveBootstrapContext,
                CertificateValidator = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.CertificateValidator,
                CertificateValidationMode = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.CertificateValidationMode
            };
        }

        /// <summary>
        /// Create claims from the token.
        /// </summary>
        /// <param name="samlToken">The token to translate to claims.</param>
        /// <returns>An identity with the created claims.</returns>
        public new ClaimsIdentity CreateClaims(Saml2SecurityToken samlToken)
        {
            var identity = base.CreateClaims(samlToken);

            if (Configuration.SaveBootstrapContext)
            {
                identity.BootstrapContext = new BootstrapContext(samlToken, this);
            }

            return identity;
        }

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
    }
}
