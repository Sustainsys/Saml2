using System.Collections.Generic;

namespace Sustainsys.Saml2.Metadata.Services
{
    /// <summary>
    /// A collection of indexed entries with support for getting the
    /// configured default entry
    /// </summary>
    /// <typeparam name="T">The type stored in the collection</typeparam>
    public class IndexedCollectionWithDefault<T> : SortedList<int, T>
        where T : class, IIndexedEntryWithDefault
    {
        public T Default
        {
            get
            {
                T possibleDefault = null;
                foreach (var endpoint in Values)
                {
                    if (endpoint.IsDefault == true)
                    {
                        return endpoint;
                    }
                    if (!endpoint.IsDefault.HasValue)
                    {
                        possibleDefault = endpoint;
                    }
                }
                return possibleDefault ?? (Count > 0 ? this[0] : null);
            }
        }
    }
}