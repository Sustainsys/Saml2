using Kentor.AuthServices.WebSso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Kentor.AuthServices.HttpModule
{
    /// <summary>
    /// Static class that hold extension methods for <see cref="HttpRequestBase"/>.
    /// </summary>
    public static class HttpRequestBaseExtensions
    {
        /// <summary>
        /// Extension method to convert a HttpRequestBase to a HttpRequestData.
        /// </summary>
        /// <param name="requestBase">The request object used to populate the <c>HttpRequestData</c>.</param>
        /// <returns>The <c>HttpRequestData</c> object that has been populated by the request.</returns>
        public static HttpRequestData ToHttpRequestData(this HttpRequestBase requestBase)
        {
            if (requestBase == null)
            {
                throw new ArgumentNullException(nameof(requestBase));
            }

            Claim nameIdentifier = null;
            var claimsIdentity = requestBase.RequestContext.HttpContext.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                nameIdentifier = claimsIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            }

            return new HttpRequestData(
                requestBase.HttpMethod,
                requestBase.Url,
                requestBase.ApplicationPath,
                requestBase.Form.Cast<string>().Select((de, i) =>
                    new KeyValuePair<string, string[]>(de, ((string)requestBase.Form[i]).Split(','))), 
                nameIdentifier);
        }
    }
}
