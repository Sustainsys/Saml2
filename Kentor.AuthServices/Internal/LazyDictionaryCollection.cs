using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Internal
{
    /// <summary>
    /// Wrapper to expose an IEnumerable&lt;KeyValuePair&gt; as a IReadOnlyDictionary
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the read-only dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the read-only dictionary.</typeparam>
    internal class LazyDictionaryCollection<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private readonly IEnumerable<KeyValuePair<TKey, TValue[]>> valueData;

        /// <summary>
        /// Initializes the LazyDictionary with data
        /// </summary>
        /// <param name="valueData"></param>
        public LazyDictionaryCollection(IEnumerable<KeyValuePair<TKey, TValue[]>> valueData)
        {
            if(valueData == null)
            {
                throw new ArgumentNullException(nameof(valueData));
            }
            this.valueData = valueData;
        }

        /// <summary>
        /// Gets the element that has the specified key in the read-only dictionary.
        /// </summary>
        /// <param name="key">Search key</param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                var valuesByKey = valueData.Where(item => key.Equals(item.Key));
                if (!valuesByKey.Any())
                {
                    throw new KeyNotFoundException(string.Format(CultureInfo.CurrentCulture, "Key {0} not found", key));
                }
                return valuesByKey.Single().Value.Single();
            }
        }

        /// <summary>
        /// The number of items in the dictionary
        /// </summary>
        public int Count
        {
            get
            {
                return valueData.Count();
            }
        }

        /// <summary>
        /// All keys in the dictionary
        /// </summary>
        public IEnumerable<TKey> Keys
        {
            get
            {
                return valueData.Select(d => d.Key);
            }
        }

        /// <summary>
        /// All Values in the dictionary
        /// </summary>
        public IEnumerable<TValue> Values
        {
            get
            {
                return valueData.Select(d => d.Value.Single());
            }
        }

        /// <summary>
        /// Determines whether the read-only dictionary contains an element that has the
        /// specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>
        /// true if the read-only dictionary contains an element that has the specified key;
        /// otherwise, false.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            return valueData.Any(d => key.Equals(d.Key));
        }

        /// <summary>
        /// Gets an enumerator for the content
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return valueData.Select(d => new KeyValuePair<TKey, TValue>(d.Key, d.Value.Single())).GetEnumerator();
        }

        /// <summary>
        /// Gets the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the
        /// key is found; otherwise, the default value for the type of the value parameter.
        /// This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements the System.Collections.Generic.IReadOnlyDictionary`2
        /// interface contains an element that has the specified key; otherwise, false.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            var valuesByKey = valueData.Where(item => key.Equals(item.Key));
            if (!valuesByKey.Any())
            {
                value = default(TValue);
                return false;
            }
            value = valuesByKey.Single().Value.Single();
            return true;
        }

        /// <summary>
        /// Gets an enumerator for the content
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
