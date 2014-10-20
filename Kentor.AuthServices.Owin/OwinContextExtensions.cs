using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Owin
{
    static class OwinContextExtensions
    {
        public async static Task<HttpRequestData> ToHttpRequestData(this IOwinContext context)
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
            return new HttpRequestData(
                context.Request.Method,
                context.Request.Uri,
                context.Request.PathBase.Value,
                formData);
        }
    }
}
