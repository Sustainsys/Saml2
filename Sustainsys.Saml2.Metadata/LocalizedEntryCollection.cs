using System.Collections.ObjectModel;

namespace Sustainsys.Saml2.Metadata
{
    public class LocalizedEntryCollection<T> : KeyedCollection<string, T> where T : LocalizedEntry
    {
        protected override string GetKeyForItem(T item)
        {
            return item.Language;
        }
    }
}