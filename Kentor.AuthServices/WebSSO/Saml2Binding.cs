using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kentor.AuthServices.WebSso
{
    /// <summary>
    /// Abstract base for all Saml2Bindings that binds a message to a specific
    /// kind of transport.
    /// </summary>
    public abstract class Saml2Binding
    {
        /// <summary>
        /// Uri identifier of the HTTP-POST binding.
        /// </summary>
        public static readonly Uri HttpPostUri = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST");

        /// <summary>
        /// Uri identifier of the HTTP-Redirect binding.
        /// </summary>
        public static readonly Uri HttpRedirectUri = new Uri("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect");

        /// <summary>
        /// Uri identifier of the Discovery Response SAML extension.
        /// </summary>
        public static readonly Uri DiscoveryResponseUri = new Uri("urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol");

        /// <summary>
        /// Bind the message to a transport.
        /// </summary>
        /// <param name="payload">(xml) payload data to bind.</param>
        /// <param name="destinationUrl">The destination of the message.</param>
        /// <param name="messageName">The name of the message to use in a query string or form input field.
        /// Typically "SAMLRequest" or "SAMLResponse".
        /// </param>
        /// <returns>CommandResult to be returned to the client browser.</returns>
        public virtual CommandResult Bind(string payload, Uri destinationUrl, string messageName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Extracts a message out of the current HttpRequest.
        /// </summary>
        /// <param name="request">Current HttpRequest.</param>
        /// <returns>Extracted message.</returns>
        public virtual string Unbind(HttpRequestData request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the binding can extract a message out of the current
        /// http request.
        /// </summary>
        /// <param name="request">HttpRequest to check for message.</param>
        /// <returns>True if the binding supports the current request.</returns>
        protected internal virtual bool CanUnbind(HttpRequestData request)
        {
            return false;
        }

        private static readonly IDictionary<Saml2BindingType, Saml2Binding> bindings =
            new Dictionary<Saml2BindingType, Saml2Binding>()
            {
                { Saml2BindingType.HttpRedirect, new Saml2RedirectBinding() },
                { Saml2BindingType.HttpPost, new Saml2PostBinding() }
            };

        /// <summary>
        /// Get a cached binding instance that supports the requested type.
        /// </summary>
        /// <param name="binding">Type of binding to get</param>
        /// <returns>A derived class instance that supports the requested binding.</returns>
        public static Saml2Binding Get(Saml2BindingType binding)
        {
            return bindings[binding];
        }

        /// <summary>
        /// Get a cached binding instance that can handle the current request.
        /// </summary>
        /// <param name="request">Current HttpRequest</param>
        /// <returns>A derived class instance that supports the requested binding,
        /// or null if no binding supports the current request.</returns>
        public static Saml2Binding Get(HttpRequestData request)
        {
            return bindings.FirstOrDefault(b => b.Value.CanUnbind(request)).Value;
        }

        private readonly static IDictionary<Uri, Saml2BindingType> bindingTypeMap = new Dictionary<Uri, Saml2BindingType>()
        {
            { HttpRedirectUri, Saml2BindingType.HttpRedirect },
            { HttpPostUri, Saml2BindingType.HttpPost }
        };

        /// <summary>
        /// Gets the Saml2BindingType enum value for a Saml2Binding type uri, where the
        /// uri should be one specified in the standard.
        /// </summary>
        /// <param name="uri">Uri for the binding.</param>
        /// <returns>Binding type enum value.</returns>
        /// <exception cref="ArgumentException">If the uri doesn't correspond to a known binding.</exception>
        public static Saml2BindingType UriToSaml2BindingType(Uri uri)
        {
            if(uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            Saml2BindingType bindingType;
            if(bindingTypeMap.TryGetValue(uri, out bindingType))
            {
                return bindingType;
            }

            var msg = string.Format(CultureInfo.InvariantCulture, "Unknown Saml2 Binding Uri \"{0}\".", uri);
            throw new ArgumentException(msg);
        }
    }
}
