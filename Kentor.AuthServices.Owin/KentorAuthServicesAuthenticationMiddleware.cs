using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security;

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
        /// <param name="app">The app that this middleware will be registered with.</param>
        /// <param name="options">Settings for the middleware.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification="options is validated by base ctor. Test case for null options giving ArgumentNullException works.")]
        public KentorAuthServicesAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app,
            KentorAuthServicesAuthenticationOptions options)
            :base (next, options)
        {
            if(app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if(string.IsNullOrEmpty(options.SignInAsAuthenticationType))
            {
                options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();
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
