using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Metadata
{
    /// <summary>
    /// This certificate validator does not rely on a certificate provided by the certificate store but instead takes 
    /// an already provided certificate from elsewhere to perform the validation against. At the moment it does not try
    /// to validate the chain of trust on either certificates.
    /// </summary>
    public class X509FromFileCertificateValidator : X509CertificateValidator
    {
        private readonly X509Certificate2 certificateFromFile;

        /// <summary>
        /// Initializes the validator with the certificate that incoming messages will be validated against. 
        /// </summary>
        /// <param name="onFileCertificate">The certificate that incoming keys will be validated against.</param>
        public X509FromFileCertificateValidator(X509Certificate2 onFileCertificate)
        {
            if (onFileCertificate == null)
            {
                throw new ArgumentNullException("onFileCertificate");
            }
            certificateFromFile = onFileCertificate;
        }

        /// <summary>
        /// Method that will validate the certificate against the validator. In this case the provided certificate.
        /// </summary>
        /// <param name="certificate">The certificate to validate.</param>
        public override void Validate(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }

            if (!certificate.Thumbprint.Equals(certificateFromFile.Thumbprint))
            {
                throw new SecurityTokenValidationException("The certificate does not match the provided certificate.");
            }
        }
    }
}
