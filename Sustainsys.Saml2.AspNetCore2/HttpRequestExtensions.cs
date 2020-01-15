using Sustainsys.Saml2.WebSso;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.AspNetCore2
{
    static class HttpRequestExtensions
    {
        public static HttpRequestData ToHttpRequestData(
            this HttpContext httpContext,
            Func<byte[], byte[]> cookieDecryptor)
        {
            var request = httpContext.Request;

            var uri = new Uri(
                request.Scheme
                + "://"
                + request.Host
                + request.Path
                + request.QueryString);

            var pathBase = httpContext.Request.PathBase.Value;
            pathBase = string.IsNullOrEmpty(pathBase) ? "/" : pathBase;
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> formData = null;
            if (httpContext.Request.Method == "POST")
            {
                formData = request.Form.Select(
                    f => new KeyValuePair<string, IEnumerable<string>>(f.Key, f.Value));
            }

            return new HttpRequestData(
                httpContext.Request.Method,
                uri,
                pathBase,
                formData,
                request.Cookies,
                cookieDecryptor,
                httpContext.User);
        }

        public static string GetUserAgent(this HttpRequest request)
        {
            return request.Headers["user-agent"].FirstOrDefault() ?? "";
        }
    }
}
