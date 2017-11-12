using Kentor.AuthServices.Metadata;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Collection of items with two sources: configured and loaded dynamically.
    /// The dynamically loaded can reset while the configured are kept.
    /// metadata.
    /// </summary>
    public  class ConfiguredAndLoadedSigningKeysCollection: IEnumerable<AsymmetricSecurityKey>
    {
        private IList<AsymmetricSecurityKey> configuredItems 
            = new List<AsymmetricSecurityKey>();
        private IList<AsymmetricSecurityKey> loadedItems 
            = new List<AsymmetricSecurityKey>();

        /// <summary>
        /// Add a configured key.
        /// </summary>
        /// <param name="key">Key to add.</param>
        public void AddConfiguredKey(AsymmetricSecurityKey key)
        {
            configuredItems.Add(key);
        }

        /// <summary>
        /// Add a configured certificate.
        /// </summary>
        /// <param name="certificate">Certificate to add.</param>
        public void AddConfiguredKey(X509Certificate2 certificate)
        {
            AddConfiguredKey(new X509SecurityKey(certificate));
        }

        /// <summary>
        /// Set the complete set of loaded items keys. Previously loaded items
        /// are cleared, configured items remain.
        /// </summary>
        /// <param name="items">Items to set</param>
        public void SetLoadedItems(IList<AsymmetricSecurityKey> items)
        {
            loadedItems = items;
        }

        /// <summary>
        /// The loaded items.
        /// </summary>
        public IEnumerable<AsymmetricSecurityKey> LoadedItems
        {
            get
            {
                return loadedItems;
            }
        }

        /// <summary>
        /// Gets an enumerator to the combined set of keys.
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<AsymmetricSecurityKey> GetEnumerator()
        {
            return configuredItems.Union(loadedItems).GetEnumerator();
        }

        // Weakly typed method only exists because interface requires it,
        // ignore in code coverage.
        [ExcludeFromCodeCoverage()]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
