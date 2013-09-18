using System;
using System.Linq;

namespace Kentor.AuthServices
{
    class IdentityProvider
    {
        public Saml2AuthenticationRequest CreateAuthenticateRequest()
        {
            return new Saml2AuthenticationRequest()
            {
                DestinationUri = DestinationUri
            };
        }

        public Uri DestinationUri { get; set; }
    }
}
