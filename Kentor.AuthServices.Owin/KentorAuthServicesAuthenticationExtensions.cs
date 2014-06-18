using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Owin
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
        /// <returns></returns>
        public static IAppBuilder UseKentorAuthServicesAuthentication(this IAppBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            app.Use(typeof(KentorAuthServicesAuthenticationMiddleware));

            return app;
        }
    }
}
