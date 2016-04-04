using Kentor.AuthServices.Internal;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config collection of ServiceCertificateElements.
    /// </summary>
    public class ServiceCertificateElementCollection : ConfigurationElementCollection, IEnumerable<ServiceCertificateElement>
    {
        /// <summary>
        /// Create new element of right type.
        /// </summary>
        /// <returns>ServiceCertificateElement</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceCertificateElement();
        }

        /// <summary>
        /// Get the name of an element.
        /// </summary>
        /// <param name="element">ServiceCertificateElement</param>
        /// <returns>element.Name</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Get a strongly typed enumerator.
        /// </summary>
        /// <returns>Strongly typed enumerator.</returns>
        public new IEnumerator<ServiceCertificateElement> GetEnumerator()
        {
            return base.GetEnumerator().AsGeneric<ServiceCertificateElement>();
        }

        /// <summary>
        /// Register the configured service certificates.
        /// </summary>
        /// <param name="options">Current options.</param>
        public void RegisterServiceCertificates(SPOptions options)
        {
            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            foreach(var serviceCertEntry in this)
            {
                options.ServiceCertificates.Add(new ServiceCertificate(serviceCertEntry));
            }
        }
    }
}
