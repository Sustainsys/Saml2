using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Net;

namespace Kentor.AuthServices.WebSso
{
    /// <summary>
    /// The sign in command. Use 
    /// CommandFactory.Get(CommandFactory.SignInCommandName) to get an instance.
    /// </summary>
    public class SignInCommand : ICommand
    {
        /// <summary>
        /// Ctor, don't want anyone to create instances.
        /// </summary>
        internal SignInCommand() { }

        /// <summary>
        /// Run the command, initiating the sign in sequence.
        /// </summary>
        /// <param name="request">Request data.</param>
        /// <param name="options">Options</param>
        /// <returns>CommandResult</returns>
        public CommandResult Run(HttpRequestData request, IOptions options)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return Run(
                new EntityId(request.QueryString["idp"].FirstOrDefault()),
                request.QueryString["ReturnUrl"].FirstOrDefault(),
                request,
                options,
				request.StoredRequestState?.RelayData);
        }

        /// <summary>
        /// Initiate the sign in sequence.
        /// </summary>
        /// <param name="idpEntityId">Entity id of idp to sign in to, or
        /// null to use default (discovery service if configured)</param>
        /// <param name="returnPath">Path to redirect to when the sign in
        /// is complete.</param>
        /// <param name="request">The incoming http request.</param>
        /// <param name="options">Options.</param>
        /// <param name="relayData">Data to store and make available when the
        /// ACS command has processed the response.</param>
        /// <returns>Command Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
        public static CommandResult Run(
            EntityId idpEntityId,
            string returnPath,
            HttpRequestData request,
            IOptions options,
            IDictionary<string, string> relayData)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var urls = new AuthServicesUrls(request, options.SPOptions);

            IdentityProvider idp = options.Notifications.SelectIdentityProvider(idpEntityId, relayData);
            if (idp == null)
            {
                if (idpEntityId?.Id == null)
                {
                    if (options.SPOptions.DiscoveryServiceUrl != null)
                    {
                        var commandResult = RedirectToDiscoveryService(returnPath, options.SPOptions, urls, relayData);
                        options.Notifications.SignInCommandResultCreated(commandResult, relayData);
                        return commandResult;
                    }
                    idp = options.IdentityProviders.Default;
                }
                else
                {
                    if (!options.IdentityProviders.TryGetValue(idpEntityId, out idp))
                    {
                        throw new InvalidOperationException("Unknown idp");
                    }
                }
            }

            var returnUrl = string.IsNullOrEmpty(returnPath)
                ? null
                : new Uri(returnPath, UriKind.RelativeOrAbsolute);

            return InitiateLoginToIdp(options, relayData, urls, idp, returnUrl);
        }

        private static CommandResult InitiateLoginToIdp(IOptions options, IDictionary<string, string> relayData, AuthServicesUrls urls, IdentityProvider idp, Uri returnUrl)
        {
            var authnRequest = idp.CreateAuthenticateRequest(urls);

            options.Notifications.AuthenticationRequestCreated(authnRequest, idp, relayData);

            var commandResult = idp.Bind(authnRequest);

            commandResult.RequestState = new StoredRequestState(
                idp.EntityId, returnUrl, authnRequest.Id, relayData);
            commandResult.SetCookieName = "Kentor." + authnRequest.RelayState;

            options.Notifications.SignInCommandResultCreated(commandResult, relayData);

            return commandResult;
        }

        private static CommandResult RedirectToDiscoveryService(
            string returnPath,
            SPOptions spOptions,
            AuthServicesUrls authServicesUrls,
            IDictionary<string, string> relayData)
        {
            string returnUrl = authServicesUrls.SignInUrl.OriginalString;

            var relayState = SecureKeyGenerator.CreateRelayState();

            if (!string.IsNullOrEmpty(returnPath))
            {
                returnUrl += "?ReturnUrl=" + Uri.EscapeDataString(returnPath);
                returnUrl += "&RelayState=" + Uri.EscapeDataString(relayState);
            }
            else
            {
                returnUrl += "?RelayState=" + Uri.EscapeDataString(relayState);
            }

            var redirectLocation = string.Format(
                CultureInfo.InvariantCulture,
                "{0}?entityID={1}&return={2}&returnIDParam=idp",
                spOptions.DiscoveryServiceUrl,
                Uri.EscapeDataString(spOptions.EntityId.Id),
                Uri.EscapeDataString(returnUrl));

            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = new Uri(redirectLocation),
                RequestState = new StoredRequestState(null, null, null, relayData),
                SetCookieName = "Kentor." + relayState
            };
        }
    }
}
