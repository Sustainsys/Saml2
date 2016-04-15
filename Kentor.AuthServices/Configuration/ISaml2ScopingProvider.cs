using System.Collections.Generic;
using Kentor.AuthServices.Saml2P;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISaml2ScopingProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authnRequest"></param>
        /// <param name="relayData"></param>
        /// <returns></returns>
        Saml2Scoping GetScoping(Saml2AuthenticationRequest authnRequest, IDictionary<string, string> relayData);
    }
}