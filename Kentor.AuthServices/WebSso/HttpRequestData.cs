using Kentor.AuthServices.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Security.Claims;

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
        /// <param name="cookies">Cookies of request</param>
        /// <param name="cookieDecryptor">Function that decrypts cookie
        /// contents to clear text.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Decryptor")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public HttpRequestData(
            string httpMethod,
            Uri url,
            string applicationPath,
            IEnumerable<KeyValuePair<string, string[]>> formData,
            IEnumerable<KeyValuePair<string, string>> cookies,
            Func<byte[], byte[]> cookieDecryptor) : this(httpMethod, url, applicationPath, formData, cookies, cookieDecryptor, user: null)
        {
            // empty
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpMethod">Http method of the request</param>
        /// <param name="url">Full url requested</param>
        /// <param name="formData">Form data, if present (only for POST requests)</param>
        /// <param name="applicationPath">Path to the application root</param>
        /// <param name="cookies">Cookies of request</param>
        /// <param name="cookieDecryptor">Function that decrypts cookie
        /// contents to clear text.</param>
        /// <param name="user">Claims Principal associated with the request</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Decryptor")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public HttpRequestData(
            string httpMethod,
            Uri url,
            string applicationPath,
            IEnumerable<KeyValuePair<string, string[]>> formData,
            IEnumerable<KeyValuePair<string, string>> cookies,
            Func<byte[], byte[]> cookieDecryptor,
            ClaimsPrincipal user)
        {
            Init(httpMethod, url, applicationPath, formData, cookies, cookieDecryptor, user);
        }

        // Used by tests.
        internal HttpRequestData(string httpMethod, Uri url)
        {
            Init(httpMethod, url, "/", null, Enumerable.Empty<KeyValuePair<string, string>>(), null, null);
        }

        // Used by tests.
        internal HttpRequestData(
            string httpMethod,
            Uri url,
            string applicationPath,
            IEnumerable<KeyValuePair<string, string[]>> formData,
            StoredRequestState storedRequestState)
        {
            InitBasicFields(httpMethod, url, applicationPath, formData);
            StoredRequestState = storedRequestState;
        }

        private void Init(
            string httpMethod,
            Uri url,
            string applicationPath,
            IEnumerable<KeyValuePair<string, string[]>> formData,
            IEnumerable<KeyValuePair<string, string>> cookies,
            Func<byte[], byte[]> cookieDecryptor,
            ClaimsPrincipal user)
        {
            InitBasicFields(httpMethod, url, applicationPath, formData);
            User = user;

            var relayState = QueryString["RelayState"].SingleOrDefault();
            if(relayState == null)
            {
                Form.TryGetValue("RelayState", out relayState);
            }
            RelayState = relayState;

            if (relayState != null)
            {
                var cookieName = "Kentor." + relayState;
                if (cookies.Any(c => c.Key == cookieName))
                {
                    var cookieData = cookies.SingleOrDefault(c => c.Key == cookieName).Value;
                    byte[] encryptedData = GetBinaryData(cookieData);

                    var decryptedData = cookieDecryptor(encryptedData);

                    StoredRequestState = new StoredRequestState(decryptedData);
                }
            }
        }

        internal static byte[] GetBinaryData(string cookieData)
        {
            return Convert.FromBase64String(
                cookieData
                .Replace('_', '/')
                .Replace('-', '+')
                .Replace('.', '='));
        }

        private void InitBasicFields(string httpMethod, Uri url, string applicationPath, IEnumerable<KeyValuePair<string, string[]>> formData)
        {
            HttpMethod = httpMethod;
            Url = url;
            ApplicationUrl = new Uri(url, applicationPath);
            Form = new ReadOnlyDictionary<string, string>(
                (formData ?? Enumerable.Empty<KeyValuePair<string, string[]>>())
                .ToDictionary(kv => kv.Key, kv => kv.Value.Single()));
            QueryString = QueryStringHelper.ParseQueryString(url.Query);
        }

        /// <summary>
        /// Escape a Base 64 encoded cookie value, matching the unescaping
        /// that is done in the ctor.
        /// </summary>
        /// <param name="data">Data to escape</param>
        /// <returns>Escaped data</returns>
        public static string ConvertBinaryData(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return Convert.ToBase64String(data)
                .Replace('/', '_')
                .Replace('+', '-')
                .Replace('=', '.');
        }

        /// <summary>
        /// The http method of the request.
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// The complete Url of the request.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// The form data associated with the request (if any).
        /// </summary>
        public IReadOnlyDictionary<string, string> Form { get; set; }

        /// <summary>
        /// The query string parameters of the request.
        /// </summary>
        public ILookup<String, String> QueryString { get; set; }

        /// <summary>
        /// The root Url of the application. This includes the virtual directory
        /// that the application is installed in, e.g. http://hosting.example.com/myapp/
        /// </summary>
        public Uri ApplicationUrl { get; set; }

        /// <summary>
        /// RelayState from SAML message
        /// </summary>
        public string RelayState { get; set; }

        /// <summary>
        /// Request state from a previous call, carried over through cookie.
        /// </summary>
        public StoredRequestState StoredRequestState { get; set; }

        /// <summary>
        /// User (if any) associated with the request
        /// </summary>
        public ClaimsPrincipal User { get; set; }
    }
}
