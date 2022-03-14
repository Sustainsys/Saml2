using System;

namespace Sustainsys.Saml2.Metadata.Tokens
{
    public class KeyNameIdentifierClause : SecurityKeyIdentifierClause
    {
        public KeyNameIdentifierClause(string keyName) :
            base(null)
        {
            KeyName = keyName;
        }

        public string KeyName { get; private set; }

        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (keyIdentifierClause == null)
            {
                throw new ArgumentNullException(nameof(keyIdentifierClause));
            }
            return keyIdentifierClause is KeyNameIdentifierClause otherClause &&
                Matches(otherClause.KeyName);
        }

        public bool Matches(string keyName)
        {
            return KeyName == keyName;
        }

        public override string ToString()
        {
            return $"KeyNameIdentifierClause(KeyName = '{KeyName}')";
        }
    }
}