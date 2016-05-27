using Kentor.AuthServices.WebSso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Kentor.AuthServices.HttpModule
{
    /// <summary>
    /// Static class that hold extension methods for <see cref="HttpRequestBase"/>.
    /// </summary>
    public static class HttpRequestBaseExtensions
    {
        internal const string ProtectionPurpose = "Kentor.AuthServices";

        /// <summary>
        /// Extension method to convert a HttpRequestBase to a HttpRequestData.
        /// </summary>
        /// <param name="requestBase">The request object used to populate the <c>HttpRequestData</c>.</param>
        /// <returns>The <c>HttpRequestData</c> object that has been populated by the request.</returns>
        public static HttpRequestData ToHttpRequestData(this HttpRequestBase requestBase)
        {
            return requestBase.ToHttpRequestData(false);
        }
        
        /// <summary>
        /// Extension method to convert a HttpRequestBase to a HttpRequestData.
        /// </summary>
        /// <param name="requestBase">The request object used to populate the <c>HttpRequestData</c>.</param>
        /// <param name="ignoreCookies">Ignore cookies when extracting data.
        /// This is useful for the stub idp that might see the relay state
        /// and the requester's cookie, but shouldn't try to decrypt it.</param>
        /// <returns>The <c>HttpRequestData</c> object that has been populated by the request.</returns>
        public static HttpRequestData ToHttpRequestData(
            this HttpRequestBase requestBase,
            bool ignoreCookies)
        {
            if (requestBase == null)
            {
                throw new ArgumentNullException(nameof(requestBase));
            }

            var cookies = ignoreCookies
                ? Enumerable.Empty<KeyValuePair<string, string>>()
                : GetCookies(requestBase);

            return new HttpRequestData(
                requestBase.HttpMethod,
                requestBase.Url,
                requestBase.ApplicationPath,
                requestBase.Form.Cast<string>().Select((de, i) =>
                    new KeyValuePair<string, string[]>(de, ((string)requestBase.Form[i]).Split(','))),
                cookies,
                v => MachineKey.Unprotect(v, ProtectionPurpose),
                ClaimsPrincipal.Current);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetCookies(HttpRequestBase requestBase)
        {
            for (int i = 0; i < requestBase.Cookies.Count; i++)
            {
                var cookie = requestBase.Cookies[i];
                yield return new KeyValuePair<string, string>(cookie.Name, cookie.Value);
            }
        }
    }
}
