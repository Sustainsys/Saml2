using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Abstract base for all Saml2Bindings that binds a message to a specific
    /// kind of transport.
    /// </summary>
    public abstract class Saml2Binding
    {
        /// <summary>
        /// Bind the message to a transport.
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns>CommandResult to be returned to the client browser.</returns>
        public virtual CommandResult Bind(ISaml2Message message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Extracts a message out of the current HttpRequest.
        /// </summary>
        /// <param name="request">Current HttpRequest.</param>
        /// <returns>Extracted message.</returns>
        public virtual TSaml2Message Unbind<TSaml2Message>(HttpRequestBase request) where TSaml2Message : class, ISaml2Message
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the binding can extract a message out of the current
        /// http request.
        /// </summary>
        /// <param name="request">HttpRequest to check for message.</param>
        /// <returns>True if the binding supports the current request.</returns>
        protected internal virtual bool CanUnbind(HttpRequestBase request)
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
        public static Saml2Binding Get(HttpRequestBase request)
        {
            return bindings.FirstOrDefault(b => b.Value.CanUnbind(request)).Value;
        }
    }
}
