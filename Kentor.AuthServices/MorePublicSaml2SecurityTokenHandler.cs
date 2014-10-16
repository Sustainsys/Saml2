﻿using Kentor.AuthServices.Configuration;
using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Security.Claims;

namespace Kentor.AuthServices
{
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
            var identity = base.CreateClaims(samlToken);

            if (Configuration.SaveBootstrapContext)
            {
                identity.BootstrapContext = new BootstrapContext(samlToken, this);
            }

            return identity;
        }

        public new void DetectReplayedToken(SecurityToken token)
        {
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
                new Uri(KentorAuthServicesSection.Current.EntityId));

            defaultInstance = new MorePublicSaml2SecurityTokenHandler
            {
                Configuration = new SecurityTokenHandlerConfiguration
                {
                    IssuerNameRegistry = new ReturnRequestedIssuerNameRegistry(),
                    AudienceRestriction = audienceRestriction,
                    SaveBootstrapContext = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SaveBootstrapContext
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
