using System;
using System.Linq;
using System.Web;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Http Module for SAML2 authentication. The module hijacks the 
    /// ~/Saml2AuthenticationModule/ path of the http application to provide 
    /// authentication services.
    /// </summary>
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
            var request = ((HttpApplication)sender).Request;

            if(request.AppRelativeCurrentExecutionFilePath
                .StartsWith(ModulePath, StringComparison.OrdinalIgnoreCase))
            {
                var moduleRelativePath = request.AppRelativeCurrentExecutionFilePath
                    .Substring(ModulePath.Length);

                CommandFactory.GetCommand(moduleRelativePath);
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
