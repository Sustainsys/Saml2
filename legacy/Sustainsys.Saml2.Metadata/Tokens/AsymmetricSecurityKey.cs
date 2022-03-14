using System.Security.Cryptography;

namespace Sustainsys.Saml2.Metadata.Tokens
{
    public abstract class AsymmetricSecurityKey : SecurityKey
    {
        public abstract AsymmetricAlgorithm GetAsymmetricAlgorithm(string algorithm, bool privateKey);

        public abstract HashAlgorithm GetHashAlgorithmForSignature(string algorithm);

        public abstract AsymmetricSignatureDeformatter GetSignatureDeformatter(string algorithm);

        public abstract AsymmetricSignatureFormatter GetSignatureFormatter(string algorithm);

        public abstract bool HasPrivateKey();
    }
}