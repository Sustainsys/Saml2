using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config collection of federations.
    /// </summary>
    public class FederationCollection : ConfigurationElementCollection, IEnumerable<FederationElement>
    {
        /// <summary>
        /// Create new elemnt of the right type.
        /// </summary>
        /// <returns>FederationElement</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new FederationElement();
        }

        /// <summary>
        /// Get the key of an element, which is the metadata url.
        /// </summary>
        /// <param name="element">FedertionElement</param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FederationElement)element).MetadataUrl.ToString();
        }

        /// <summary>
        /// Generic IEnumerable implementation.
        /// </summary>
        /// <returns>Enumerator</returns>
        public new IEnumerator<FederationElement> GetEnumerator()
        {
            return base.GetEnumerator().AsGeneric<FederationElement>();
        }

        /// <summary>
        /// Registers the identity providers from the configured federations in the identity provider dictionary.
        /// </summary>
        /// <param name="identityProviderDictionary"></param>
        public void RegisterFederations(IdentityProviderDictionary identityProviderDictionary)
        {
            if(identityProviderDictionary == null)
            {
                throw new ArgumentNullException("identityProviderDictionary");
            }

            foreach(var configFederation in this)
            {
                var federation = new Federation(configFederation);

                foreach(var idp in federation.IdentityProviders)
                {
                    identityProviderDictionary[idp.EntityId] = idp;
                }
            }
        }
    }
}
