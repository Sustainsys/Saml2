using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Sustainsys.Saml2.AspNetCore2
{
    /// <summary>
    /// Options for Saml2 Authentication
    /// </summary>
    public class Saml2Options : RemoteAuthenticationOptions
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public Saml2Options()
        {
            CallbackPath = new PathString("/Saml2/Acs");
        }
    }
}