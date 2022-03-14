using System;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;

namespace Sustainsys.Saml2.Metadata.Tokens
{
    public abstract class SecurityToken
    {
        protected SecurityToken()
        {
        }

        public abstract DateTime ValidFrom { get; }
        public abstract DateTime ValidTo { get; }
        public abstract string Id { get; }
        public abstract ReadOnlyCollection<SecurityKey> SecurityKeys { get; }

        public virtual bool CanCreateKeyIdentifierClause<T>() where T : SecurityKeyIdentifierClause
        {
            throw new NotImplementedException();
        }

        public virtual T CreateKeyIdentifierClause<T>() where T : SecurityKeyIdentifierClause
        {
            throw new NotImplementedException();
        }

        public virtual bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            return false;
        }

        public virtual SecurityKey ResolveKeyIdentifierClause(
            SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (keyIdentifierClause == null)
            {
                throw new ArgumentNullException(nameof(keyIdentifierClause));
            }
            if (!MatchesKeyIdentifierClause(keyIdentifierClause))
            {
                throw new InvalidOperationException($"This '{GetType()}' security token does not support resolving '{keyIdentifierClause}' key identifier clause.");
            }
            if (keyIdentifierClause.CanCreateKey)
            {
                return keyIdentifierClause.CreateKey();
            }
            // FIXME: examine it.
            if (SecurityKeys.Count == 0)
            {
                throw new InvalidOperationException($"This '{GetType()}' security token does not have any keys that can be resolved.");
            }
            return SecurityKeys[0];
        }
    }

    public class X509SecurityToken : SecurityToken, IDisposable
    {
        private X509Certificate2 certificate;

        public X509Certificate2 Certificate
        {
            get
            {
                CheckDisposed();
                return certificate;
            }
        }

        private string id;

        public override string Id
        {
            get
            {
                CheckDisposed();
                return id;
            }
        }

        public X509SecurityToken(X509Certificate2 certificate, string id)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }
            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            this.id = id;
            this.certificate = certificate;
        }

        public X509SecurityToken(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }
            this.certificate = certificate;
            this.id = certificate.IssuerName.Name + certificate.SerialNumber;
        }

        private void CheckDisposed()
        {
            if (certificate == null)
            {
                throw new ObjectDisposedException("X509SecurityToken");
            }
        }

        public override DateTime ValidFrom
        {
            get
            {
                CheckDisposed();
                return Certificate.NotBefore.ToUniversalTime();
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                CheckDisposed();
                return Certificate.NotAfter.ToUniversalTime();
            }
        }

        public virtual void Dispose()
        {
            CheckDisposed();

            certificate.Dispose();
            certificate = null;
        }

        private ReadOnlyCollection<SecurityKey> keys;

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                CheckDisposed();

                if (keys == null)
                {
                    keys = new ReadOnlyCollection<SecurityKey>(new SecurityKey[] { new X509AsymmetricSecurityKey(Certificate) });
                }
                return keys;
            }
        }

        public override bool CanCreateKeyIdentifierClause<T>()
        {
            CheckDisposed();

            Type t = typeof(T);
            return t == typeof(X509RawDataKeyIdentifierClause) ||
                   t == typeof(X509IssuerSerialKeyIdentifierClause);
            // X509SubjectKeyIdentifierClause, X509ThumbprintKeyIdentifierClause
        }

        public override T CreateKeyIdentifierClause<T>()
        {
            CheckDisposed();

            Type t = typeof(T);
            if (t == typeof(X509RawDataKeyIdentifierClause))
            {
                return (T)(object)new X509RawDataKeyIdentifierClause(certificate);
            }
            if (t == typeof(X509IssuerSerialKeyIdentifierClause))
            {
                return (T)(object)new X509IssuerSerialKeyIdentifierClause(certificate);
            }
            throw new NotSupportedException($"A key identifier of type {t} could not be created");
        }

        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            CheckDisposed();

            // TODO:
            // LocalIdKeyIdentifierClause , X509ThumbprintKeyIdentifierClause ,
            // X509SubjectKeyIdentifierClause
            if (keyIdentifierClause is X509IssuerSerialKeyIdentifierClause isk)
            {
                return isk.Matches(certificate);
            }
            if (keyIdentifierClause is X509RawDataKeyIdentifierClause rdk)
            {
                return rdk.Matches(certificate);
            }
            return false;
        }
    }
}