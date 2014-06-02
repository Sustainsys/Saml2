using Kentor.AuthServices.Configuration;
using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Claims;

namespace Kentor.AuthServices
{
    using System.IdentityModel.Services;

    /// <summary>
    /// Somewhat ugly subclassing to be able to access some methods that are protected
    /// on Saml2SecurityTokenHandler. The public interface of Saml2SecurityTokenHandler
    /// expects the actual assertion to be signed, which is not the case for us since
    /// the entire response message is signed for us instead.
    /// </summary>
    class MorePublicSaml2SecurityTokenHandler : Saml2SecurityTokenHandler
    {
        public new ClaimsIdentity CreateClaims(Saml2SecurityToken samlToken)
        {
            return base.CreateClaims(samlToken);
        }

        public new void DetectReplayedToken(SecurityToken token)
        {
            if (!FederatedAuthentication.FederationConfiguration.IdentityConfiguration.DetectReplayedTokens)
            {
                return;
            }

            base.DetectReplayedToken(token);
        }

        public new void ValidateConditions(Saml2Conditions conditions, bool enforceAudienceRestriction)
        {
            base.ValidateConditions(conditions, enforceAudienceRestriction);
        }
        
        private static readonly MorePublicSaml2SecurityTokenHandler defaultInstance;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static MorePublicSaml2SecurityTokenHandler()
        {
            var audienceRestriction = new AudienceRestriction(AudienceUriMode.Always);
            audienceRestriction.AllowedAudienceUris.Add(
                new Uri(KentorAuthServicesSection.Current.Issuer));

            defaultInstance = new MorePublicSaml2SecurityTokenHandler()
            {
                Configuration = new SecurityTokenHandlerConfiguration()
                {
                    IssuerNameRegistry = new ReturnRequestedIssuerNameRegistry(),
                    AudienceRestriction = audienceRestriction
                }
            };
        }

        /// <summary>
        /// Get a default of the class, with a default implementation.
        /// </summary>
        public static MorePublicSaml2SecurityTokenHandler DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
        }
    }
}
