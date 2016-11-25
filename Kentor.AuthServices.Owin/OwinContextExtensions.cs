using Kentor.AuthServices.WebSso;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Owin
{
    static class OwinContextExtensions
    {
        public async static Task<HttpRequestData> ToHttpRequestData(
            this IOwinContext context,
            Func<byte[], byte[]> cookieDecryptor)
        {
            if(context == null)
            {
                return null;
            }

            IFormCollection formData = null;
            if(context.Request.Body != null)
            {
                formData = await context.Request.ReadFormAsync();
            }

            var applicationRootPath = context.Request.PathBase.Value;
            if(string.IsNullOrEmpty(applicationRootPath))
            {
                applicationRootPath = "/";
            }

            return new HttpRequestData(
                context.Request.Method,
                context.Request.Uri,
                applicationRootPath,
                formData,
                context.Request.Cookies,
                cookieDecryptor,
                context.Request.User as ClaimsPrincipal);
        }
    }
}
