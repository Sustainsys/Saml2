using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Kentor.AuthServices.WebSso;

namespace Sustainsys.Saml2.AspNetCore2
{
    /// <summary>
    /// Authentication handler for Saml2
    /// </summary>
    public class Saml2Handler : RemoteAuthenticationHandler<Saml2Options>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="logger">Logger</param>
        /// <param name="encoder">Encoder</param>
        /// <param name="clock">Clock</param>
        public Saml2Handler(
            IOptionsMonitor<Saml2Options> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
        }

        /// <InheritDocs />
        protected override Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        /// <InheritDocs />
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var requestData = Context.ToHttpRequestData(null);

            var result = CommandFactory.GetCommand(CommandFactory.SignInCommandName)
                .Run(requestData, Options);

            result.Apply(Context);

            return Task.CompletedTask;
        }
    }
}
