using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Internal
{
    /// <summary>
    /// Class implements static methods to help parse a query string.
    /// </summary>
    internal static class QueryStringHelper
    {
        /// <summary>
        /// Splits a query string into its key/value pairs. 
        /// </summary>
        /// <param name="queryString">A query string, with or without the leading '?' character.</param>
        /// <returns>A collecktion with the parsed keys and values.</returns>
        public static ILookup<String, String> ParseQueryString(String queryString)
        {
            if (queryString == null)
            {
                throw new ArgumentNullException(nameof(queryString));
            }

            if (queryString.Length != 0 && queryString[0] == '?')
            {
                queryString = queryString.Substring(1);
            }

            return queryString.Split('&')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x =>
                {
                    int indexOfFirstEqualsSign = x.IndexOf("=");
                    if(indexOfFirstEqualsSign == -1)
                    {
                        return new string[] { x };
                    }
                    return new string[] 
                    {
                        x.Substring(0, indexOfFirstEqualsSign),
                        x.Substring(indexOfFirstEqualsSign + 1)
                    };
                })
                .ToLookup(y => y[0], y => y.Length > 1 ? Uri.UnescapeDataString(y[1]) : null);
        }
    }
}
