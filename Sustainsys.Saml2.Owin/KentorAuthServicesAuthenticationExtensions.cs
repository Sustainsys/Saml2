using Kentor.AuthServices.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin
{
    /// <summary>
    /// Extension method to easily attach Kentor AuthServices to the Owin pipeline.
    /// </summary>
    public static class KentorAuthServicesAuthenticationExtensions
    {
        /// <summary>
        /// Add Kentor AuthServices SAML2 authentication to the Owin pipeline.
        /// </summary>
        /// <param name="app">Owin pipeline builder.</param>
        /// <param name="options">Options for the middleware.</param>
        /// <returns></returns>
        public static IAppBuilder UseKentorAuthServicesAuthentication(this IAppBuilder app, 
            KentorAuthServicesAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.Use(typeof(KentorAuthServicesAuthenticationMiddleware), app, options);

            return app;
        }
    }
}
