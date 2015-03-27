using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using Kentor.AuthServices.Configuration;

namespace Kentor.AuthServices.Internal
{
    /// <summary>
    /// A security token resolver that resolves by
    /// looking up the Issuer in the configured
    /// list of Identity Providers
    /// </summary>
    class IdpTokenResolver : SecurityTokenResolver
    {
        private readonly IdentityProviderDictionary identityProviders;
        private readonly SecurityTokenResolver secondaryResolver;

        public IdpTokenResolver(IdentityProviderDictionary identityProviders)
            : this( identityProviders, new IssuerTokenResolver())
        {
        }

        public IdpTokenResolver(IdentityProviderDictionary identityProviders, SecurityTokenResolver secondaryResolver)
        {
            this.identityProviders = identityProviders;
            this.secondaryResolver = secondaryResolver;
        }

        protected override bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
        {
            key = null;
            var token = getToken(keyIdentifierClause);
            if (token != null && token.SecurityKeys.Count > 0)
            {
                key = token.SecurityKeys[0];
                return true;
            }

            if (secondaryResolver.TryResolveSecurityKey(keyIdentifierClause, out key))
            {
                return true;
            }

            return false;
        }

        protected override bool TryResolveTokenCore(SecurityKeyIdentifier keyIdentifier, out SecurityToken token)
        {
            token = null;
            if (keyIdentifier != null && keyIdentifier.Count > 0)
            {
                token = getToken(keyIdentifier[0]);
                return true;
            }

            if (secondaryResolver.TryResolveToken(keyIdentifier, out token))
            {
                return true;
            }

            return false;
        }

        protected override bool TryResolveTokenCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token)
        {
            throw new NotImplementedException();
        }

        private SecurityToken getToken(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            var clause = keyIdentifierClause as Saml2SecurityKeyIdentifierClause;
            if (clause == null) return null;

            var idp = identityProviders[new EntityId(clause.Assertion.Issuer.Value)];
            if (idp == null || idp.SecurityToken == null ) return null;

            return idp.SecurityToken;
        }
    }
}
