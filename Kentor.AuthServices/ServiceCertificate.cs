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
            Status = CertificateStatus.Current;
            MetadataPublishOverride = MetadataPublishOverrideType.None;
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
            Status = serviceCertElement.Status;
            MetadataPublishOverride = serviceCertElement.MetadataPublishOverride;
            Certificate = serviceCertElement.LoadCertificate();
        }

        /// <summary>
        /// X509 Certificate
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Is this certificate for current or future use?
        /// </summary>
        public CertificateStatus Status { get; set; }

        /// <summary>
        /// What is the intended use of this certificate.
        /// </summary>
        public CertificateUse Use { get; set; }

        /// <summary>
        /// How should we override the metadata publishing rules?
        /// </summary>
        public MetadataPublishOverrideType MetadataPublishOverride { get; set; }
    }
}