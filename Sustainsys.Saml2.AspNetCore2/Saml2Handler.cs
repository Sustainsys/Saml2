using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Kentor.AuthServices.WebSso;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;

namespace Sustainsys.Saml2.AspNetCore2
{
    /// <summary>
    /// Authentication handler for Saml2
    /// </summary>
    public class Saml2Handler : RemoteAuthenticationHandler<Saml2Options>
    {
        IDataProtector dataProtector;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="logger">Logger</param>
        /// <param name="encoder">Encoder</param>
        /// <param name="clock">Clock</param>
        /// <param name="dataProtectorProvider">Data Protector Provider</param>
        public Saml2Handler(
            IOptionsMonitor<Saml2Options> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IDataProtectionProvider dataProtectorProvider) 
            : base(options, logger, encoder, clock)
        {
            if(dataProtectorProvider == null)
            {
                throw new ArgumentNullException(nameof(dataProtectorProvider));
            }

            dataProtector = dataProtectorProvider.CreateProtector(GetType().FullName);
        }

        /// <InheritDocs />
        protected override Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            var requestData = Context.ToHttpRequestData(dataProtector.Unprotect);

            var commandResult = CommandFactory.GetCommand(CommandFactory.AcsCommandName)
                .Run(requestData, Options);

            var properties = new AuthenticationProperties(commandResult.RelayData)
            {
                RedirectUri = commandResult.Location.OriginalString
            };

            var ticket = new AuthenticationTicket(
                commandResult.Principal,
                properties,
                Scheme.Name);
            return Task.FromResult(HandleRequestResult.Success(ticket));
        }

        /// <InheritDocs />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Upstream caller ensures it's not null")]
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var requestData = Context.ToHttpRequestData(null);

            // Don't serialize the return url twice, move it to our location.
            var redirectUri = properties.RedirectUri;
            properties.RedirectUri = null;

            var result = SignInCommand.Run(
                null,
                redirectUri,
                requestData,
                Options,
                properties.Items);

            result.Apply(Context, dataProtector);

            return Task.CompletedTask;
        }

        /// <InheritDocs />
        public override Task<bool> ShouldHandleRequestAsync()
        {
            var acsPath = Options.SPOptions.ModulePath + "/" + CommandFactory.AcsCommandName;
            return Task.FromResult(acsPath == Request.Path);
        }
    }
}
