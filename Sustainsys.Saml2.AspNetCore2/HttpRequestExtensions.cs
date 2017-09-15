using Kentor.AuthServices.WebSso;
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
                + request.Path);

            var pathBase = httpContext.Request.PathBase.Value;
            pathBase = pathBase == "" ? "/" : pathBase;

            return new HttpRequestData(
                httpContext.Request.Method,
                uri,
                pathBase,
                request.Form.Select(f => new KeyValuePair<string, IEnumerable<string>>(f.Key, f.Value)),
                null,
                cookieDecryptor);
        }
    }
}
