using System.Collections.Generic;
using Kentor.AuthServices.Saml2P;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Extension point for providing scoping information to Saml2AuthenticationRequests.
    /// </summary>
    public interface ISaml2ScopingProvider
    {
        /// <summary>
        /// Build the relevant scoping given the authentication request and relay data.
        /// </summary>
        Saml2Scoping GetScoping(Saml2AuthenticationRequest authenticationRequest, IDictionary<string, string> relayData);
    }
}