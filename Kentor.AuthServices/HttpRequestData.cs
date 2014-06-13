using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kentor.AuthServices
{
    /// <summary>
    /// The data of a http request that AuthServices needs to handle. A separate DTO is used
    /// to make the core library totally independent of the hosting environment.
    /// </summary>
    public class HttpRequestData
    {
        /// <summary>
        /// Create a HttpRequestData from a HttpRequestBase.
        /// </summary>
        /// <param name="request">HttpRequestBase with source data.</param>
        public HttpRequestData(HttpRequestBase request)
            : this(request.HttpMethod, request.Url, 
            request.Form.Cast<DictionaryEntry>().Select(de => 
                new KeyValuePair<string, string[]>((string)de.Key, ((string)de.Value).Split(','))))
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        internal HttpRequestData(string httpMethod, Uri url, 
            IEnumerable<KeyValuePair<string, string[]>> formData = null)
        {
            HttpMethod = httpMethod;
            Url = url;
            Form = new ReadOnlyDictionary<string, string>(formData.ToDictionary(kv => kv.Key, kv => kv.Value.Single()));
            QueryString = HttpUtility.ParseQueryString(url.Query);
        }

        /// <summary>
        /// The http method of the request.
        /// </summary>
        public string HttpMethod { get; private set; }

        /// <summary>
        /// The Url of the 
        /// </summary>
        public Uri Url { get; private set; }

        /// <summary>
        /// The form data associated with the request (if any).
        /// </summary>
        public IReadOnlyDictionary<string, string> Form { get; private set; }

        /// <summary>
        /// The query string parameters of the request.
        /// </summary>
        public NameValueCollection QueryString { get; private set; }
    }
}
