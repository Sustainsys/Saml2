using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Internal
{
    /// <summary>
    /// Class implements static methods to help parse a query string.
    /// </summary>
    public static class QueryStringHelper
    {
        /// <summary>
        /// Exists because of code coverage. If the string is empty we want to create an empty instance 
        /// of Lookup. Because only internal calls can create an Lookup like:
        /// Enumerable.Empty&lt;String&gt;().ToLookup(x => default(String));
        /// 
        /// Because this ToLookup demands a lambda/delegate that never executes we get problems with code coverage. 
        /// 
        /// If we switch to a lambda without anonymous types we can use the same call to ToLookup for them both at the bottom. 
        /// </summary>
        private struct Item
        {
            public String key;
            public String value;
        }

        /// <summary>
        /// Splits a query string into its key/value pairs. 
        /// </summary>
        /// <param name="queryString">A query string, with or without the leading '?' character.</param>
        /// <returns>A collecktion with the parsed keys and values.</returns>
        public static ILookup<String, String> ParseQueryString(String queryString)
        {
            IEnumerable<Item> result;
            if (queryString == null)
            {
                throw new ArgumentNullException("queryString");
            }

            if (queryString.Length == 0)
            {
                result = Enumerable.Empty<Item>();
            }
            else
            {
                if (queryString[0] == '?')
                {
                    queryString = queryString.Substring(1);
                }

                result = queryString.Split('&')
                .Select(x => x.Split('='))
                .Select(y => new Item { key = y[0], value = Uri.UnescapeDataString(y[1]) });
            }

            return result.ToLookup(x => x.key, y => y.value);
        }
    }
}
