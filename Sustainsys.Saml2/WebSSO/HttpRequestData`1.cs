using Sustainsys.Saml2.Internal;
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

namespace Sustainsys.Saml2.WebSso
{
    /// <inheritdoc/>
    public class HttpRequestData<T> : HttpRequestData
    {
        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Decryptor")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public HttpRequestData(
            T context,
            string httpMethod,
            Uri url,
            string applicationPath,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> formData,
            IEnumerable<KeyValuePair<string, string>> cookies,
            Func<byte[], byte[]> cookieDecryptor)
            : base(
                httpMethod,
                url,
                applicationPath,
                formData,
                cookies,
                cookieDecryptor)
        {
            Context = context;
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Decryptor")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public HttpRequestData(
            T context,
            string httpMethod,
            Uri url,
            string applicationPath,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> formData,
            IEnumerable<KeyValuePair<string, string>> cookies,
            Func<byte[], byte[]> cookieDecryptor,
            ClaimsPrincipal user)
            : base(
                httpMethod,
                url,
                applicationPath,
                formData,
                cookies,
                cookieDecryptor,
                user)
        {
            Context = context;
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Decryptor")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public HttpRequestData(
            T context,
            string httpMethod,
            Uri url,
            string applicationPath,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> formData,
            Func<string, string> cookieReader,
            Func<byte[], byte[]> cookieDecryptor,
            ClaimsPrincipal user)
            : base(
                httpMethod,
                url,
                applicationPath,
                formData,
                cookieReader,
                cookieDecryptor,
                user)
        {
            Context = context;
        }

        // Used by tests.
        internal HttpRequestData(
            T context,
            string httpMethod, 
            Uri url)
            : base(httpMethod, url)
        {
            Context = context;
        }

        // Used by tests.
        internal HttpRequestData(
            T context,
            string httpMethod,
            Uri url,
            string applicationPath,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> formData,
            StoredRequestState storedRequestState)
            : base(
                httpMethod,
                url,
                applicationPath,
                formData,
                storedRequestState)
        {
            Context = context;
        }

        /// <summary>
        /// The typed target-specific HTTP request.
        /// </summary>
        public T Context;

        /// <inheritdoc/>
        public override object ContextObject => Context;
    }
}
