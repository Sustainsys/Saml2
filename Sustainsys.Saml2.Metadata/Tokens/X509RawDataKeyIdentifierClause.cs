using System;
using System.Security.Cryptography.X509Certificates;

namespace Sustainsys.Saml2.Metadata.Tokens
{
    public class X509RawDataKeyIdentifierClause : BinaryKeyIdentifierClause
    {
        private X509Certificate2 certificate;

        public X509RawDataKeyIdentifierClause(byte[] certificateRawData) :
            base(null, certificateRawData, true)
        {
        }

        public X509RawDataKeyIdentifierClause(X509Certificate2 certificate) :
            base(null, certificate.RawData, true)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }
            this.certificate = certificate;
        }

        public override bool CanCreateKey => true;

        public override SecurityKey CreateKey()
        {
            if (certificate == null)
            {
                certificate = new X509Certificate2(GetX509RawData());
            }
            return new X509AsymmetricSecurityKey(certificate);
        }

        public byte[] GetX509RawData()
        {
            return GetRawBuffer();
        }

        public bool Matches(X509Certificate2 otherCert)
        {
            return Matches(otherCert.RawData);
        }
    }
}