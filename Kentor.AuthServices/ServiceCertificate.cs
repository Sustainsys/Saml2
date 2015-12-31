using Kentor.AuthServices.Configuration;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Service Certificate definition
    /// </summary>
    public class ServiceCertificate
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ServiceCertificate()
        {
            Use = CertificateUse.Both;
            Active = true;
        }

        /// <summary>
        /// Ctor for loading from configuration
        /// </summary>
        /// <param name="serviceCertElement"></param>
        public ServiceCertificate(ServiceCertificateElement serviceCertElement)
        {
            if (serviceCertElement == null)
            {
                throw new ArgumentNullException(nameof(serviceCertElement));
            }
            Use = serviceCertElement.Use;
            Active = serviceCertElement.Active;
            Certificate = serviceCertElement.Certificate.LoadCertificate();
        }

        /// <summary>
        /// X509 Certificate
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Should this certificate be used for new protocol transactions.
        /// Inactive certificates are still valid until expired or removed from configuration.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// What is the intended use of this certificate.
        /// </summary>
        public CertificateUse Use { get; set; }
    }
}