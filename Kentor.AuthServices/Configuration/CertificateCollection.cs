using Sustainsys.Saml2.Internal;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Sustainsys.Saml2.Configuration
{
    /// <summary>
    /// Collection of certificate elements.
    /// </summary>
    public class CertificateCollection : ConfigurationElementCollection, IEnumerable<CertificateElement>
    {
        /// <summary>
        /// Create a new element of the right type.
        /// </summary>
        /// <returns>A new certificate element</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new CertificateElement();
        }

        /// <summary>
        /// Get the key of an element.
        /// </summary>
        /// <param name="element">Element to get key of.</param>
        /// <returns>A guid. There is no support for removing items and we
        /// want this to be unique.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Get enumerator for the elements.
        /// </summary>
        /// <returns></returns>
        public new IEnumerator<CertificateElement> GetEnumerator()
        {
            return base.GetEnumerator().AsGeneric<CertificateElement>();
        }

    }
}