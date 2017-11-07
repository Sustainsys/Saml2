using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace Kentor.AuthServices.Metadata
{
    public class X509CertificateKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        public X509Certificate2 Certificate { get; }

        public X509CertificateKeyIdentifierClause(X509Certificate2 certificate)
        {
            Certificate = certificate;
        }

        public AsymmetricSecurityKey CreateKey()
        {
            return new X509SecurityKey(Certificate);
        }
    }
}