using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Web;

namespace Kentor.AuthServices
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

        const string ModulePath = "~/Saml2AuthenticationModule/";

        /// <summary>
        /// Begin request handler that captures all traffic to ~/Saml2AuthenticationModule/
        /// </summary>
        /// <param name="sender">The http application.</param>
        /// <param name="e">Ignored</param>
        protected virtual void OnBeginRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;

            if(application.Request.AppRelativeCurrentExecutionFilePath
                .StartsWith(ModulePath, StringComparison.OrdinalIgnoreCase))
            {
                var moduleRelativePath = application.Request.AppRelativeCurrentExecutionFilePath
                    .Substring(ModulePath.Length);

                var command = CommandFactory.GetCommand(moduleRelativePath);
                var commandResult = RunCommand(application, command);

                commandResult.ApplyPrincipal();
                commandResult.Apply(new HttpResponseWrapper(application.Response));
            }
        }
 
        private static CommandResult RunCommand(HttpApplication application, ICommand command)
        {
            try
            {
                return command.Run(new HttpRequestWrapper(application.Request));
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
