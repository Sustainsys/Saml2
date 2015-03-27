using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace Kentor.AuthServices.Internal
{
    /// <summary>
    /// A security token resolver that always resolves to the token given
    /// in the constructor. Use e.g. if the token for the identity provider
    /// is already known from our configuration
    /// </summary>
    class KnownTokenResolver : SecurityTokenResolver
    {
        private readonly SecurityToken knownToken;

        public KnownTokenResolver(SecurityToken knownToken)
        {
            this.knownToken = knownToken;
        }

        protected override bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
        {
            key = knownToken.SecurityKeys[ 0 ];
            return true;
        }

        protected override bool TryResolveTokenCore(SecurityKeyIdentifier keyIdentifier, out SecurityToken token)
        {
            token = knownToken;
            return true;
        }

        protected override bool TryResolveTokenCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token)
        {
            token = knownToken;
            return true;
        }
    }
}
