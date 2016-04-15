using System.Collections.Generic;
using Kentor.AuthServices.Saml2P;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class SingleSaml2ScopingProvider : ISaml2ScopingProvider
    {
        private readonly Saml2Scoping _saml2Scoping;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saml2Scoping"></param>
        public SingleSaml2ScopingProvider(Saml2Scoping saml2Scoping)
        {
            _saml2Scoping = saml2Scoping;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authnRequest"></param>
        /// <param name="relayData"></param>
        /// <returns></returns>
        public Saml2Scoping GetScoping(Saml2AuthenticationRequest authnRequest, IDictionary<string, string> relayData)
        {
            return _saml2Scoping;
        }
    }
}