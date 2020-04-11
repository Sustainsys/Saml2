using Sustainsys.Saml2.Metadata.Selectors;
using Sustainsys.Saml2.Metadata.Tokens;
using System;
using System.Xml;
using SecurityToken = Microsoft.IdentityModel.Tokens.SecurityToken;

namespace Sustainsys.Saml2.Metadata.Serialization
{
    public abstract class SecurityTokenSerializer
    {
        protected abstract bool CanReadTokenCore(XmlReader reader);

        protected abstract bool CanWriteTokenCore(SecurityToken token);

        protected abstract bool CanReadKeyIdentifierCore(XmlReader reader);

        protected abstract bool CanWriteKeyIdentifierCore(SecurityKeyIdentifier keyIdentifier);

        protected abstract bool CanReadKeyIdentifierClauseCore(XmlReader reader);

        protected abstract bool CanWriteKeyIdentifierClauseCore(SecurityKeyIdentifierClause keyIdentifierClause);

        protected abstract SecurityToken ReadTokenCore(XmlReader reader, SecurityTokenResolver tokenResolver);

        protected abstract void WriteTokenCore(XmlWriter writer, SecurityToken token);

        protected abstract SecurityKeyIdentifier ReadKeyIdentifierCore(XmlReader reader);

        protected abstract void WriteKeyIdentifierCore(XmlWriter writer, SecurityKeyIdentifier keyIdentifier);

        protected abstract SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(XmlReader reader);

        protected abstract void WriteKeyIdentifierClauseCore(XmlWriter writer, SecurityKeyIdentifierClause keyIdentifierClause);

        private void CheckReader(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
        }

        public bool CanReadKeyIdentifier(XmlReader reader)
        {
            CheckReader(reader);
            return CanReadKeyIdentifierCore(reader);
        }

        public bool CanReadKeyIdentifierClause(XmlReader reader)
        {
            CheckReader(reader);
            return CanReadKeyIdentifierClauseCore(reader);
        }

        public bool CanReadToken(XmlReader reader)
        {
            CheckReader(reader);
            return CanReadTokenCore(reader);
        }

        public bool CanWriteKeyIdentifier(SecurityKeyIdentifier keyIdentifier)
        {
            if (keyIdentifier == null)
            {
                throw new ArgumentNullException(nameof(keyIdentifier));
            }
            return CanWriteKeyIdentifierCore(keyIdentifier);
        }

        public bool CanWriteKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (keyIdentifierClause == null)
            {
                throw new ArgumentNullException(nameof(keyIdentifierClause));
            }
            return CanWriteKeyIdentifierClauseCore(keyIdentifierClause);
        }

        public bool CanWriteToken(SecurityToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            return CanWriteTokenCore(token);
        }

        public SecurityKeyIdentifier ReadKeyIdentifier(XmlReader reader)
        {
            CheckReader(reader);
            return ReadKeyIdentifierCore(reader);
        }

        public SecurityKeyIdentifierClause ReadKeyIdentifierClause(XmlReader reader)
        {
            CheckReader(reader);
            return ReadKeyIdentifierClauseCore(reader);
        }

        public SecurityToken ReadToken(XmlReader reader, SecurityTokenResolver resolver)
        {
            CheckReader(reader);
            return ReadTokenCore(reader, resolver);
        }

        private void CheckWriter(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
        }

        public void WriteKeyIdentifier(XmlWriter writer, SecurityKeyIdentifier keyIdentifier)
        {
            CheckWriter(writer);
            if (keyIdentifier == null)
            {
                throw new ArgumentNullException(nameof(keyIdentifier));
            }
            WriteKeyIdentifierCore(writer, keyIdentifier);
        }

        public void WriteKeyIdentifierClause(XmlWriter writer, SecurityKeyIdentifierClause keyIdentifierClause)
        {
            CheckWriter(writer);
            if (keyIdentifierClause == null)
            {
                throw new ArgumentNullException(nameof(keyIdentifierClause));
            }
            WriteKeyIdentifierClauseCore(writer, keyIdentifierClause);
        }

        public void WriteToken(XmlWriter writer, SecurityToken token)
        {
            CheckWriter(writer);
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            WriteTokenCore(writer, token);
        }
    }
}