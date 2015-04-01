using Kentor.AuthServices.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Linq;

namespace Kentor.AuthServices.WebSso
{
    /// <summary>
    /// The data of a http request that AuthServices needs to handle. A separate DTO is used
    /// to make the core library totally independent of the hosting environment.
    /// </summary>
    public class HttpRequestData
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpMethod">Http method of the request</param>
        /// <param name="url">Full url requested</param>
        /// <param name="formData">Form data, if present (only for POST requests)</param>
        /// <param name="applicationPath">Path to the application root</param>
        /// <param name="nameIdentifier">The claim that contains the nameIdentifier of the logged in user</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures"), SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public HttpRequestData(
            string httpMethod,
            Uri url,
            string applicationPath,
            IEnumerable<KeyValuePair<string, string[]>> formData,
            Claim nameIdentifier = null)
        {
            Init(httpMethod, url, applicationPath, formData, nameIdentifier);
        }

        // Used by tests.
        internal HttpRequestData(string httpMethod, Uri url)
        {
            Init(httpMethod, url, "/", null, null);
        }

        private void Init(
            string httpMethod,
            Uri url,
            string applicationPath,
            IEnumerable<KeyValuePair<string, string[]>> formData,
            Claim nameIdentifier)
        {
            HttpMethod = httpMethod;
            Url = url;
            ApplicationUrl = new Uri(url, applicationPath);
            Form = new ReadOnlyDictionary<string, string>(
                (formData ?? Enumerable.Empty<KeyValuePair<string, string[]>>())
                .ToDictionary(kv => kv.Key, kv => kv.Value.Single()));
            QueryString = QueryStringHelper.ParseQueryString(url.Query);
            NameIdentifier = nameIdentifier;
        }

        /// <summary>
        /// The http method of the request.
        /// </summary>
        public string HttpMethod { get; private set; }

        /// <summary>
        /// The complete Url of the request.
        /// </summary>
        public Uri Url { get; private set; }

        /// <summary>
        /// The form data associated with the request (if any).
        /// </summary>
        public IReadOnlyDictionary<string, string> Form { get; private set; }

        /// <summary>
        /// The query string parameters of the request.
        /// </summary>
        public ILookup<String, String> QueryString { get; private set; }

        /// <summary>
        /// The root Url of the application. This includes the virtual directory
        /// that the application is installed in, e.g. http://hosting.example.com/myapp/
        /// </summary>
        public Uri ApplicationUrl { get; private set; }

        /// <summary>
        /// The Claim that contains the nameidentifier of the user that is currently logged in
        /// </summary>
        public Claim NameIdentifier { get; private set; }
    }
}
