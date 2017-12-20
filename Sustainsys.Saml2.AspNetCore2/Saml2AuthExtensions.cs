using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.AspNetCore2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions methods for adding Saml2 authentication
    /// </summary>
    public static class Saml2AuthExtensions
    {
        /// <summary>
        /// Register Saml2 Authentication with default scheme name.
        /// </summary>
        /// <param name="builder">Authentication Builder</param>
        /// <param name="configureOptions">Action that configures the Saml2 Options</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddSaml2(
            this AuthenticationBuilder builder,
            Action<Saml2Options> configureOptions)
            => builder.AddSaml2(Saml2Defaults.Scheme, configureOptions);

        /// <summary>
        /// Register Saml2 Authentication with a custom scheme name.
        /// </summary>
        /// <param name="builder">Authentication Builder</param>
        /// <param name="scheme">Name of the authentication scheme</param>
        /// <param name="configureOptions">Action that configures Saml2 Options</param>
        /// <returns>Authentication Builder</returns>
        public static AuthenticationBuilder AddSaml2(
            this AuthenticationBuilder builder,
            string scheme,
            Action<Saml2Options> configureOptions)
            => builder.AddSaml2(scheme, Saml2Defaults.DisplayName, configureOptions);

        /// <summary>
        /// Register Saml2 Authentication with a custom scheme name.
        /// </summary>
        /// <param name="builder">Authentication Builder</param>
        /// <param name="scheme">Name of the authentication scheme</param>
        /// <param name="configureOptions">Action that configures Saml2 Options</param>
        /// <param name="displayName">Display name of scheme</param>
        /// <returns>Authentication Builder</returns>
        public static AuthenticationBuilder AddSaml2(
            this AuthenticationBuilder builder,
            string scheme,
            string displayName,
            Action<Saml2Options> configureOptions)
        {
            if(builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<Saml2Options>, PostConfigureSaml2Options>());

            builder.Services.Configure<AuthenticationOptions>(o =>
            {
                o.AddScheme(scheme, s =>
                {
                    s.HandlerType = typeof(Saml2Handler);
                    s.DisplayName = displayName;
                });
            });
            
            if (configureOptions != null)
            {
                builder.Services.Configure(scheme, configureOptions);
            }

            builder.Services.AddTransient<Saml2Handler>();

            return builder;
        }
    }
}
