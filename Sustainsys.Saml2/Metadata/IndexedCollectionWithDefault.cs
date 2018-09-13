using System;
using System.Collections.Generic;
using System.Text;

namespace Sustainsys.Saml2.Metadata
{
	/// <summary>
	/// An indexed entry with an optional default
	/// </summary>
	public interface IIndexedEntryWithDefault
	{
		/// <summary>
		/// Index of the endpoint
		/// </summary>
		int Index { get; set; }

		/// <summary>
		/// Is this the default endpoint?
		/// </summary>
		bool? IsDefault { get; set; }
	}

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
