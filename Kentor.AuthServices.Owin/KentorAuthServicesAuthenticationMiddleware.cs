using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Owin
{
    /// <summary>
    /// Owin middleware for SAML2 authentication.
    /// </summary>
    public class KentorAuthServicesAuthenticationMiddleware 
        : AuthenticationMiddleware<KentorAuthServicesAuthenticationOptions>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="options">Settings for the middleware.</param>
        public KentorAuthServicesAuthenticationMiddleware(OwinMiddleware next,
            KentorAuthServicesAuthenticationOptions options)
            :base (next, options)
        {
            if(options == null)
            {
                throw new ArgumentNullException("options");
            }

            if(string.IsNullOrEmpty(options.AuthenticationType))
            {
                options.AuthenticationType = Constants.DefaultAuthenticationType;
            }
        }

        /// <summary>
        /// Creates a handler instance for use when processing a request.
        /// </summary>
        /// <returns>Handler instance.</returns>
        protected override AuthenticationHandler<KentorAuthServicesAuthenticationOptions> CreateHandler()
        {
            return new KentorAuthServicesAuthenticationHandler();
        }
    }
}
