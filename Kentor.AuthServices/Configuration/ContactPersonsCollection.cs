using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Kentor.AuthServices.Internal;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config collection of contacts.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class ContactPersonsCollection : ConfigurationElementCollection, IEnumerable<ContactPersonElement>
    {
        /// <summary>
        /// Create a new element of the right type.
        /// </summary>
        /// <returns>A new ContactPersonElement.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ContactPersonElement();
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
        public new IEnumerator<ContactPersonElement> GetEnumerator()
        {
            return base.GetEnumerator().AsGeneric<ContactPersonElement>();
        }
    }
}
