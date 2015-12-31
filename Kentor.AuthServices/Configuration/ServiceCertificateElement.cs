using System.Configuration;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config element for the service certificate element.
    /// </summary>
    public class ServiceCertificateElement : ConfigurationElement
    {
        /// <summary>
        /// Should this be used for future transactions
        /// </summary>
        [ConfigurationProperty("active", IsRequired = false, DefaultValue = true)]
        public bool Active
        {
            get
            {
                return (bool)base["active"];
            }
            internal set
            {
                base["active"] = value;
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
            internal set
            {
                base["use"] = value;
            }
        }

        /// <summary>
        /// Certificate location
        /// </summary>
        [ConfigurationProperty("certificate")]
        public CertificateElement Certificate
        {
            get
            {
                return (CertificateElement)base["certificate"];
            }
            internal set
            {
                base["certificate"] = value;
            }
        }
    }
}
