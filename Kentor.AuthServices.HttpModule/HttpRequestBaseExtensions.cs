using Kentor.AuthServices.WebSso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kentor.AuthServices.HttpModule
{
    public static class HttpRequestBaseExtensions
    {
        public static HttpRequestData ToHttpRequestData(this HttpRequestBase requestBase)
        {
            if (requestBase == null)
            {
                throw new ArgumentNullException("requestBase");
            }

            return new HttpRequestData(
                requestBase.HttpMethod,
                requestBase.Url,
                requestBase.ApplicationPath,
                requestBase.Form.Cast<string>().Select((de, i) =>
                    new KeyValuePair<string, string[]>(de, ((string)requestBase.Form[i]).Split(','))));
        }
    }
}
