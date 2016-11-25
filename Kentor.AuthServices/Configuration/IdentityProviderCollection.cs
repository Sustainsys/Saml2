using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kentor.AuthServices.Internal;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config collection of IdentityProviderElements.
    /// </summary>
    public class IdentityProviderCollection : ConfigurationElementCollection, IEnumerable<IdentityProviderElement>
    {
        /// <summary>
        /// Create new element of right type.
        /// </summary>
        /// <returns>IdentityProviderElement</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new IdentityProviderElement();
        }

        /// <summary>
        /// Get the name of an element.
        /// </summary>
        /// <param name="element">IdentityProviderElement</param>
        /// <returns>element.Name</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IdentityProviderElement)element).EntityId;
        }

        /// <summary>
        /// Get a strongly typed enumerator.
        /// </summary>
        /// <returns>Strongly typed enumerator.</returns>
        public new IEnumerator<IdentityProviderElement> GetEnumerator()
        {
            return base.GetEnumerator().AsGeneric<IdentityProviderElement>();
        }

        /// <summary>
        /// Register the configured identity providers in the dictionary of active idps.
        /// </summary>
        /// <param name="options">Current options.</param>
        public void RegisterIdentityProviders(IOptions options)
        {
            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            foreach(var idpEntry in this)
            {
                var idp = new IdentityProvider(idpEntry, options.SPOptions);
                
                options.IdentityProviders[idp.EntityId] = idp;
            }
        }
    }
}
