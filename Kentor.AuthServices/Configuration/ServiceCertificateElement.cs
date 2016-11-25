using System.Configuration;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config element for the service certificate element.
    /// </summary>
    public class ServiceCertificateElement : CertificateElement
    {
        /// <summary>
        /// Is this certificate for current or future use?
        /// </summary>
        [ConfigurationProperty("status", IsRequired = false, DefaultValue = CertificateStatus.Current)]
        public CertificateStatus Status
        {
            get
            {
                return (CertificateStatus)base["status"];
            }
        }

        /// <summary>
        /// Intended use of the certificate
        /// </summary>
        [ConfigurationProperty("use", IsRequired = false, DefaultValue = CertificateUse.Both)]
        public CertificateUse Use
        {
            get
            {
                return (CertificateUse)base["use"];
            }
        }

        /// <summary>
        /// How should we override the metadata publishing rules?
        /// </summary>
        [ConfigurationProperty("metadataPublishOverride", IsRequired = false, DefaultValue = MetadataPublishOverrideType.None)]
        public MetadataPublishOverrideType MetadataPublishOverride
        {
            get
            {
                return (MetadataPublishOverrideType)base["metadataPublishOverride"];
            }
        }
    }
}
