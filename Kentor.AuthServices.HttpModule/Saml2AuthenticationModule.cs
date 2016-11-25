using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.WebSso;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Web;

namespace Kentor.AuthServices.HttpModule
{
    /// <summary>
    /// Http Module for SAML2 authentication. The module hijacks the 
    /// ~/Saml2AuthenticationModule/ path of the http application to provide 
    /// authentication services.
    /// </summary>
    // Not included in code coverage as the http module is tightly dependent on IIS.
    [ExcludeFromCodeCoverage]
    public class Saml2AuthenticationModule : IHttpModule
    {
        /// <summary>
        /// The one and only options instance used by the
        /// <see cref="Saml2AuthenticationModule"/>. It is instantiated by
        /// loading the web.config, but after that it can be modified or even
        /// replaced from code.
        /// </summary>
        public static IOptions Options { get; set; } = Configuration.Options.FromConfiguration;

        /// <summary>
        /// Init the module and subscribe to events.
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Run our code post authentication to allow any session authentication
            // to be done first (required by logout) but still execute as close
            // as possible to the normal authentication step.
            context.AuthenticateRequest += OnAuthenticateRequest;
        }

        /// <summary>
        /// Begin request handler that captures all traffic to configured module
        /// path.
        /// </summary>
        /// <param name="sender">The http application.</param>
        /// <param name="e">Ignored</param>
        protected void OnAuthenticateRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;

            // Strip the leading ~ from the AppRelative path.
            var appRelativePath = application.Request.AppRelativeCurrentExecutionFilePath;
            appRelativePath = (!string.IsNullOrEmpty(appRelativePath))
                ? appRelativePath.Substring(1)
                : string.Empty;

            var modulePath = Options.SPOptions.ModulePath;

            if (appRelativePath.StartsWith(modulePath, StringComparison.OrdinalIgnoreCase))
            {
                var commandName = appRelativePath.Substring(modulePath.Length);

                var command = CommandFactory.GetCommand(commandName);
                var commandResult = command.Run(
                    new HttpRequestWrapper(application.Request).ToHttpRequestData(),
                    Options);

                if (!commandResult.HandledResult)
                {
                    commandResult.SignInOrOutSessionAuthenticationModule();
                    commandResult.Apply(new HttpResponseWrapper(application.Response));
                }
            }
        }

        /// <summary>
        /// IDisposable implementation.
        /// </summary>
        public virtual void Dispose()
        {
            // Deliberately do nothing, unsubscribing from events is not
            // needed by the IIS model. Trying to do so throws exceptions.
        }
    }
}
