using System;
using System.Collections.Generic;
using System.Text;

namespace Sustainsys.Saml2.Metadata
{
	public interface IIndexedEntryWithDefault
	{
		int Index { get; }
		bool? IsDefault { get; }
	}

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
