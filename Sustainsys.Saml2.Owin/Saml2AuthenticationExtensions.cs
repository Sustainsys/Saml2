using Sustainsys.Saml2.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin
{
    /// <summary>
    /// Extension method to easily attach Sustainsys Saml2 to the Owin pipeline.
    /// </summary>
    public static class Saml2AuthenticationExtensions
    {
        /// <summary>
        /// Add Sustainsys Saml2 SAML2 authentication to the Owin pipeline.
        /// </summary>
        /// <param name="app">Owin pipeline builder.</param>
        /// <param name="options">Options for the middleware.</param>
        /// <returns></returns>
        public static IAppBuilder UseSaml2Authentication(this IAppBuilder app, 
            Saml2AuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.Use(typeof(Saml2AuthenticationMiddleware), app, options);

            return app;
        }
    }
}
