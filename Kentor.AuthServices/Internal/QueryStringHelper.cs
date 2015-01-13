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
        /// Splits a query string into its key/value pairs. 
        /// </summary>
        /// <param name="queryString">A query string, with or without the leading '?' character.</param>
        /// <returns>A collecktion with the parsed keys and values.</returns>
        public static NameValueCollection ParseQueryString(String queryString)
        {
            if (queryString == null)
            {
                throw new ArgumentNullException("queryString");
            }

            NameValueCollection keyValueCollection = new NameValueCollection();

            if (queryString.Length == 0)
            {
                return keyValueCollection;
            }

            if (queryString[0] == '?')
            {
                queryString = queryString.Substring(1);
            }

            var keyValuePair = queryString.Split('&')
                .Select(x => x.Split('='))
                .Select(y => new { Key = y[0], Value = Uri.UnescapeDataString(y[1]) });

            foreach (var pair in keyValuePair)
            {
                keyValueCollection[pair.Key] = pair.Value;
            }
            return keyValueCollection;
        }


        /// <summary>
        /// Encodes an url string and acts as a replacement for HttpUtility.UrlEncode.
        /// </summary>
        /// <param name="url">The url to be encoded.</param>
        /// <returns>An encoded url.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification="This is a drop-in replacement for a microsoft library function."), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static String UrlEncode(string url)
        {
            // The Uri.EscapeDataString is almost an 100% valid replacement for 
            // HttpUtility.UrlEncode. The main difference seems to be that it does not encode 
            // + signs which EscapeDataString does, so we revert it. 
            return Uri.EscapeDataString(url).Replace("%20", "+");
        }
    }
}
