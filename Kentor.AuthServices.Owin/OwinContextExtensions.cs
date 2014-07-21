using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Owin
{
    static class OwinContextExtensions
    {
        public static HttpRequestData ToHttpRequestData(this IOwinContext context)
        {
            if(context == null)
            {
                return null;
            }

            IFormCollection formData = null;
            if(context.Request.Body != null)
            {
                formData = context.Request.ReadFormAsync().Result;
            }
            return new HttpRequestData(context.Request.Method, context.Request.Uri, formData);
        }
    }
}
