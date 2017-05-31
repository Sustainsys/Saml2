using Kentor.AuthServices.WebSso;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kentor.AuthServices.AspNetCore
{
    public static class HttpContextExtensions
    {
        public async static Task<HttpRequestData> ToHttpRequestDataAsync(
            this HttpContext context,
            Func<byte[], byte[]> cookieDecryptor)
        {
            if(context == null)
            {
                return null;
            }

            IEnumerable<KeyValuePair<string, string[]>> formData = null;
            if(context.Request.Body != null && context.Request.Method.ToUpper() == "POST")
            {
                var formCollection = await context.Request.ReadFormAsync();
                formData = formCollection.Select(d => new KeyValuePair<string, string[]>(d.Key, d.Value.ToArray()));
            }

            var applicationRootPath = context.Request.PathBase.Value;
            if(string.IsNullOrEmpty(applicationRootPath))
            {
                applicationRootPath = "/";
            }

            var url = new Uri(string.Concat(
                        context.Request.Scheme,
                        "://",
                        context.Request.Host.ToUriComponent(),
                        context.Request.PathBase.ToUriComponent(),
                        context.Request.Path.ToUriComponent(),
                        context.Request.QueryString.ToUriComponent()));

            return new HttpRequestData(
                context.Request.Method,
                url,
                applicationRootPath,
                formData,
                context.Request.Cookies,
                cookieDecryptor,
                context.User);
        }
    }
}
