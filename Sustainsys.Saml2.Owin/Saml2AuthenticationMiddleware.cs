using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.DataHandler;
using System.Configuration;
using Microsoft.Owin.Logging;

namespace Sustainsys.Saml2.Owin
{
    /// <summary>
    /// Owin middleware for SAML2 authentication.
    /// </summary>
    public class Saml2AuthenticationMiddleware 
        : AuthenticationMiddleware<Saml2AuthenticationOptions>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="app">The app that this middleware will be registered with.</param>
        /// <param name="options">Settings for the middleware.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "options is validated by base ctor. Test case for null options giving ArgumentNullException works.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SPOptions")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "EntityId")]
        public Saml2AuthenticationMiddleware(
            OwinMiddleware next,
            IAppBuilder app,
            Saml2AuthenticationOptions options)
            :base (next, options)
        {
            if(app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options.SPOptions == null)
            {
                throw new ConfigurationErrorsException("The options.SPOptions property cannot be null. There is an implementation class Sustainsys.Saml2.Configuration.SPOptions that you can instantiate. The EntityId property of that class is mandatory. It must be set to the EntityId used to represent this system.");
            }

            if (options.SPOptions.EntityId == null)
            {
                throw new ConfigurationErrorsException("The SPOptions.EntityId property cannot be null. It must be set to the EntityId used to represent this system.");
            }

            if (string.IsNullOrEmpty(options.SignInAsAuthenticationType))
            {
                options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();
            }

            options.DataProtector = app.CreateDataProtector(
                typeof(Saml2AuthenticationMiddleware).FullName, options.SPOptions.ModulePath);

            if(options.SPOptions.Logger == null)
            {
                options.SPOptions.Logger = new OwinLoggerAdapter(app.CreateLogger<Saml2AuthenticationMiddleware>());
            }
        }

        /// <summary>
        /// Creates a handler instance for use when processing a request.
        /// </summary>
        /// <returns>Handler instance.</returns>
        protected override AuthenticationHandler<Saml2AuthenticationOptions> CreateHandler()
        {
            return new Saml2AuthenticationHandler();
        }
    }
}
