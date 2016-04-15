using System.Collections.Generic;
using Kentor.AuthServices.Saml2P;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Applies a single static scoping to all autentication requests.
    /// </summary>
    public class SingleSaml2ScopingProvider : ISaml2ScopingProvider
    {
        private readonly Saml2Scoping _saml2Scoping;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleSaml2ScopingProvider"/> class. 
        /// </summary>
        /// <param name="saml2Scoping">The scoping that will always be applied.</param>
        public SingleSaml2ScopingProvider(Saml2Scoping saml2Scoping)
        {
            _saml2Scoping = saml2Scoping;
        }

        /// <summary>
        /// Build the relevant scoping given the authentication request and relay data.
        /// </summary>
        public Saml2Scoping GetScoping(Saml2AuthenticationRequest authenticationRequest, IDictionary<string, string> relayData)
        {
            return _saml2Scoping;
        }
    }
}