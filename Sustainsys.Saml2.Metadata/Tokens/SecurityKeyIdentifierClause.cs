using Microsoft.IdentityModel.Tokens;
using System;

namespace Sustainsys.Saml2.Metadata.Tokens
{
    public abstract class SecurityKeyIdentifierClause
    {
        public string ClauseType { get; private set; }
        public string Id { get; private set; }
        public int DerivationLength { get; private set; }
        private byte[] derivationNonce;

        public byte[] GetDerivationNonce()
        {
            return derivationNonce?.CloneByteArray();
        }

        public virtual bool CanCreateKey
        {
            get { return false; }
        }

        public virtual SecurityKey CreateKey()
        {
            throw new NotSupportedException("SecurityKeyIdentifierClause does not support key creation");
        }

        public virtual bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            return ReferenceEquals(this, keyIdentifierClause);
        }

        protected SecurityKeyIdentifierClause(string clauseType, byte[] nonce, int length)
        {
            ClauseType = clauseType;
            DerivationLength = length;
            derivationNonce = nonce;
        }

        protected SecurityKeyIdentifierClause(string clauseType) :
            this(clauseType, null, 0)
        {
        }
    }
}