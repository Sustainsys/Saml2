using Sustainsys.Saml2.Metadata.Configuration;
using Sustainsys.Saml2.Metadata.Tokens;
using System;
using System.Xml;

namespace Sustainsys.Saml2.Metadata.Selectors
{
    public abstract class SecurityTokenResolver : ICustomIdentityConfiguration
    {
        public SecurityToken ResolveToken(SecurityKeyIdentifier keyIdentifier)
        {
            if (keyIdentifier == null)
            {
                throw new ArgumentNullException(nameof(keyIdentifier));
            }
            SecurityToken token;
            if (!TryResolveTokenCore(keyIdentifier, out token))
            {
                throw new InvalidOperationException($"Unable to resolve token for key identifier {keyIdentifier}");
            }
            return token;
        }

        public bool TryResolveToken(SecurityKeyIdentifier keyIdentifier, out SecurityToken token)
        {
            if (keyIdentifier == null)
            {
                throw new ArgumentNullException(nameof(keyIdentifier));
            }
            return TryResolveTokenCore(keyIdentifier, out token);
        }

        public SecurityToken ResolveToken(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (keyIdentifierClause == null)
            {
                throw new ArgumentNullException(nameof(keyIdentifierClause));
            }
            SecurityToken token;
            if (!TryResolveTokenCore(keyIdentifierClause, out token))
            {
                throw new InvalidOperationException($"Unable to resolve token for key identifier clause {keyIdentifierClause}");
            }
            return token;
        }

        public bool TryResolveToken(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token)
        {
            if (keyIdentifierClause == null)
            {
                throw new ArgumentNullException(nameof(keyIdentifierClause));
            }
            return TryResolveTokenCore(keyIdentifierClause, out token);
        }

        public SecurityKey ResolveSecurityKey(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (keyIdentifierClause == null)
            {
                throw new ArgumentNullException(nameof(keyIdentifierClause));
            }
            SecurityKey key;
            if (!TryResolveSecurityKeyCore(keyIdentifierClause, out key))
            {
                throw new InvalidOperationException($"Unable to resolve key for key identifier clause {keyIdentifierClause}");
            }
            return key;
        }

        public bool TryResolveSecurityKey(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
        {
            if (keyIdentifierClause == null)
            {
                throw new ArgumentNullException(nameof(keyIdentifierClause));
            }
            return TryResolveSecurityKeyCore(keyIdentifierClause, out key);
        }

        public virtual void LoadCustomConfiguration(XmlNodeList nodeList)
        {
            throw new NotImplementedException();
        }

        protected abstract bool TryResolveTokenCore(SecurityKeyIdentifier keyIdentifier, out SecurityToken token);

        protected abstract bool TryResolveTokenCore(SecurityKeyIdentifierClause keyIdentifier, out SecurityToken token);

        protected abstract bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifier, out SecurityKey key);
    }
}