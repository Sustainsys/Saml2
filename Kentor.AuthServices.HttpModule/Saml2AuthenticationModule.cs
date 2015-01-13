using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.WebSso;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
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
        /// Init the module and subscribe to events.
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            context.BeginRequest += OnBeginRequest;
        }

        /// <summary>
        /// Begin request handler that captures all traffic to ~/Saml2AuthenticationModule/
        /// </summary>
        /// <param name="sender">The http application.</param>
        /// <param name="e">Ignored</param>
        protected virtual void OnBeginRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;

            // Strip the leading ~ from the AppRelative path.
            var appRelativePath = application.Request.AppRelativeCurrentExecutionFilePath.Substring(1);
            var modulePath = Options.FromConfiguration.SPOptions.ModulePath;

            if(appRelativePath.StartsWith(modulePath, StringComparison.OrdinalIgnoreCase))
            {
                var commandName = appRelativePath.Substring(modulePath.Length);

                var command = CommandFactory.GetCommand(commandName);
                var commandResult = RunCommand(application, command);

                commandResult.SignInSessionAuthenticationModule();
                commandResult.Apply(new HttpResponseWrapper(application.Response));
            }
        }
 
        private static CommandResult RunCommand(HttpApplication application, ICommand command)
        {
            try
            {
                return command.Run(
                    new HttpRequestWrapper(application.Request).ToHttpRequestData(),
                    Options.FromConfiguration);
            }
            catch (AuthServicesException)
            {
                return new CommandResult()
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError
                };
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
