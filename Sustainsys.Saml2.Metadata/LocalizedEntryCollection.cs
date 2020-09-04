using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sustainsys.Saml2.Metadata
{
    public class LocalizedEntryCollection<T>  : IEnumerable<T>
        where T : LocalizedEntry
    {
        private List<T> items = new List<T>();

        public void Add(T item) => items.Add(item);

        public void Clear() => items.Clear();

        public int Count => items.Count;

        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
    }
}