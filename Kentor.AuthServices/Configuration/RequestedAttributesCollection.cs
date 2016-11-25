using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Kentor.AuthServices.Internal;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Collection of requested attributes that an SP wants in incoming assertions.
    /// </summary>
    public class RequestedAttributesCollection : ConfigurationElementCollection, IEnumerable<RequestedAttributeElement>
    {
        /// <summary>
        /// Create a new element of the right type.
        /// </summary>
        /// <returns>A new RequestedAttributeElement</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new RequestedAttributeElement();
        }

        /// <summary>
        /// Get the key of an element.
        /// </summary>
        /// <param name="element">Element to get key of.</param>
        /// <returns>The name of the requested attribute.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RequestedAttributeElement)element).Name;
        }

        /// <summary>
        /// Get a generic enumerator to the collection.
        /// </summary>
        /// <returns>Generic enumerator</returns>
        public new IEnumerator<RequestedAttributeElement> GetEnumerator()
        {
            return base.GetEnumerator().AsGeneric<RequestedAttributeElement>();
        }
    }
}
