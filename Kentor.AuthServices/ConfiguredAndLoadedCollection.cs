using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Collection of items with two sources: configured and loaded dyanically.
    /// The dynamically loaded can reset while the configured are kept.
    /// metadata.
    /// </summary>
    public  class ConfiguredAndLoadedCollection<T>: IEnumerable<T>
    {
        private IList<T> configuredItems = new List<T>();
        private IList<T> loadedItems = new List<T>();

        /// <summary>
        /// Add a configured item.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void AddConfiguredItem(T item)
        {
            configuredItems.Add(item);
        }

        /// <summary>
        /// Set the complete set of loaded items keys. Previously loaded items
        /// are cleared, configured items remain.
        /// </summary>
        /// <param name="items">Items to set</param>
        public void SetLoadedItems(IList<T> items)
        {
            loadedItems = items;
        }

        /// <summary>
        /// The loaded items.
        /// </summary>
        public IEnumerable<T> LoadedItems
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
        public IEnumerator<T> GetEnumerator()
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
