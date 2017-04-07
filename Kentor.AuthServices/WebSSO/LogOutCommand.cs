using System;
using Kentor.AuthServices.Configuration;
using System.IdentityModel.Metadata;
using System.Security.Claims;
using System.Net;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.Exceptions;
using System.Globalization;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.Xml;

namespace Kentor.AuthServices.WebSso
{
    /// <summary>
    /// The logout command. Use 
    /// CommandFactory.Get(CommandFactory.LogoutCommandName) to get an instance.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout")]
    public class LogoutCommand : ICommand
    {
        /// <summary>
        /// Ctor, don't want anyone to create instances.
        /// </summary>
        internal LogoutCommand() { }

        /// <summary>
        /// Run the command, initiating or handling the logout sequence.
        /// </summary>
        /// <param name="request">Request data.</param>
        /// <param name="options">Options</param>
        /// <returns>CommandResult</returns>
        public CommandResult Run(HttpRequestData request, IOptions options)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var returnUrl = request.QueryString["ReturnUrl"].SingleOrDefault();
            Uri parsedUri;
            if (returnUrl != null && !Uri.TryCreate(returnUrl, UriKind.Relative, out parsedUri))
            {
                if (!options.Notifications.ValidateAbsoluteReturnUrl(returnUrl))
                {
                    throw new InvalidOperationException("Return Url must be a relative Url.");
                }
            }

            return Run(request, returnUrl, options);
        }

        /// <summary>
        /// Run the command, initating or handling the logout sequence.
        /// </summary>
        /// <param name="request">Request data.</param>
        /// <param name="returnPath">Path to return to, only used if this
        /// is the start of an SP-initiated logout.</param>
        /// <param name="options">Options</param>
        /// <returns>CommandResult</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
        public static CommandResult Run(
            HttpRequestData request,
            string returnPath,
            IOptions options)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            CommandResult commandResult;
            var returnUrl = GetReturnUrl(request, returnPath, options);
            var binding = options.Notifications.GetBinding(request);
            if (binding != null)
            {
                var unbindResult = binding.Unbind(request, options);
                options.Notifications.MessageUnbound(unbindResult);

                VerifyMessageIsSigned(unbindResult, options);
                switch (unbindResult.Data.LocalName)
                {
                    case "LogoutRequest":
                        commandResult = HandleRequest(unbindResult, options);
                        break;
                    case "LogoutResponse":
                        var storedRequestState = options.Notifications.GetLogoutResponseState(request);
                        commandResult = HandleResponse(unbindResult, storedRequestState, options, returnUrl);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                commandResult = InitiateLogout(request, returnUrl, options);
            }
            options.Notifications.LogoutCommandResultCreated(commandResult);
            return commandResult;
        }

        private static void VerifyMessageIsSigned(UnbindResult unbindResult, IOptions options)
        {
            if (unbindResult.TrustLevel < TrustLevel.Signature)
            {
                var issuer = unbindResult.Data["Issuer", Saml2Namespaces.Saml2Name]?.InnerText;

                if(issuer == null)
                {
                    throw new InvalidSignatureException("There is no Issuer element in the message, so there is no way to know what certificate to use to validate the signature.");
                }
                var idp = options.IdentityProviders[new EntityId(issuer)];

                if (!unbindResult.Data.IsSignedByAny(
                    idp.SigningKeys,
                    options.SPOptions.ValidateCertificates,
                    options.SPOptions.MinIncomingSigningAlgorithm))
                {
                    throw new UnsuccessfulSamlOperationException(string.Format(CultureInfo.InvariantCulture,
                        "Received a {0} from {1} that cannot be processed because it is not signed.",
                        unbindResult.Data.LocalName,
                        unbindResult.Data["Issuer", Saml2Namespaces.Saml2Name].InnerText));
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "signingCertificate")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SingleLogoutServiceUrl")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SPOptions")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "LogoutNameIdentifier")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DisableOutboundLogoutRequests")]
        private static CommandResult InitiateLogout(HttpRequestData request, Uri returnUrl, IOptions options)
        {
            string idpEntityId = null;
            Claim sessionIndexClaim = null;
            if (request.User != null)
            {
                idpEntityId = request.User.FindFirst(AuthServicesClaimTypes.LogoutNameIdentifier)?.Issuer;
                sessionIndexClaim = request.User.FindFirst(AuthServicesClaimTypes.SessionIndex);
            }

            IdentityProvider idp;
            var knownIdp = options.IdentityProviders.TryGetValue(new EntityId(idpEntityId), out idp);

            options.SPOptions.Logger.WriteVerbose("Initiating logout, checking requirements for federated logout"
                + "\n  Issuer of LogoutNameIdentifier claim (should be Idp entity id): " + idpEntityId
                + "\n  Issuer is a known Idp: " + knownIdp
                + "\n  Session index claim (should have a value): " + sessionIndexClaim
                + "\n  Idp has SingleLogoutServiceUrl: " + idp?.SingleLogoutServiceUrl?.OriginalString
                + "\n  There is a signingCertificate in SPOptions: " + (options.SPOptions.SigningServiceCertificate != null)
                + "\n  Idp configured to DisableOutboundLogoutRequests (should be false): " + idp?.DisableOutboundLogoutRequests);

            CommandResult commandResult;
            if(idpEntityId != null 
                && knownIdp
                && sessionIndexClaim != null
                && idp.SingleLogoutServiceUrl != null
                && options.SPOptions.SigningServiceCertificate != null
                && !idp.DisableOutboundLogoutRequests)
            {
                var logoutRequest = idp.CreateLogoutRequest(request.User);

                commandResult = Saml2Binding.Get(idp.SingleLogoutServiceBinding)
                    .Bind(logoutRequest);

                commandResult.RelayState = logoutRequest.RelayState;
                commandResult.RequestState = new StoredRequestState(
                    idp.EntityId,
                    returnUrl,
                    logoutRequest.Id,
                    null);

                if (!options.SPOptions.Compatibility.DisableLogoutStateCookie)
                {
                    commandResult.SetCookieName = "Kentor." + logoutRequest.RelayState;
                }

                options.SPOptions.Logger.WriteInformation("Sending logout request to " + idp.EntityId.Id);
            }
            else
            {
                commandResult = new CommandResult
                {
                    HttpStatusCode = HttpStatusCode.SeeOther,
                    Location = returnUrl
                };
                options.SPOptions.Logger.WriteInformation("Doing a local only logout.");
            }

            commandResult.TerminateLocalSession = true;

            return commandResult;
        }

        private static Uri GetReturnUrl(HttpRequestData request, string returnPath, IOptions options)
        {
            if (!string.IsNullOrEmpty(returnPath))
            {
                return new Uri(returnPath, UriKind.RelativeOrAbsolute);
            }
            else
            {
                return new AuthServicesUrls(request, options).ApplicationUrl;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SingleLogoutServiceUrl")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SingleLogoutService")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "LogoutRequest")]
        private static CommandResult HandleRequest(UnbindResult unbindResult, IOptions options)
        {
            var request = Saml2LogoutRequest.FromXml(unbindResult.Data);

            var idp = options.IdentityProviders[request.Issuer];

            if(options.SPOptions.SigningServiceCertificate == null)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture,
                    "Received a LogoutRequest from \"{0}\" but cannot reply because single logout responses " +
                    "must be signed and there is no signing certificate configured. Looks like the idp is " +
                    "configured for Single Logout despite AuthServices not exposing that functionality in the metadata.",
                    request.Issuer.Id));
            }

            if(idp.SingleLogoutServiceResponseUrl == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "Received a LogoutRequest from \"{0}\" but cannot reply because on logout endpoint is " +
                    "configured on the idp. Set a SingleLogoutServiceUrl if the idp is configured manually, " +
                    "or check that the idp metadata contains a SingleLogoutService endpoint.",
                    idp.EntityId.Id));
            }

            var response = new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = idp.SingleLogoutServiceResponseUrl,
                SigningCertificate = options.SPOptions.SigningServiceCertificate,
                SigningAlgorithm = idp.OutboundSigningAlgorithm,
                InResponseTo = request.Id,
                Issuer = options.SPOptions.EntityId,
                RelayState = unbindResult.RelayState
            };

            options.SPOptions.Logger.WriteInformation("Got a logout request " + request.Id
                + ", responding with logout response " + response.Id);

            var result = Saml2Binding.Get(idp.SingleLogoutServiceBinding).Bind(response);
            result.TerminateLocalSession = true;
            return result;
        }

        private static CommandResult HandleResponse(UnbindResult unbindResult, StoredRequestState storedRequestState, IOptions options, Uri returnUrl)
        {
            var logoutResponse = Saml2LogoutResponse.FromXml(unbindResult.Data);
            var notificationHandledTheStatus = options.Notifications.ProcessSingleLogoutResponseStatus(logoutResponse, storedRequestState);
            if (!notificationHandledTheStatus) { 
                var status = logoutResponse.Status;
                if(status != Saml2StatusCode.Success)
                {
                    throw new UnsuccessfulSamlOperationException(string.Format(CultureInfo.InvariantCulture,
                        "Idp returned status \"{0}\", indicating that the single logout failed. The local session has been successfully terminated.",
                        status));
                }
            }

            var commandResult = new CommandResult
            {
                HttpStatusCode = HttpStatusCode.SeeOther
            };
            if (!options.SPOptions.Compatibility.DisableLogoutStateCookie)
            {
                commandResult.ClearCookieName = "Kentor." + unbindResult.RelayState;
            }
            commandResult.Location = storedRequestState?.ReturnUrl ?? returnUrl;

            options.SPOptions.Logger.WriteInformation("Received logout response " + logoutResponse.Id
                + ", redirecting to " + commandResult.Location);

            return commandResult;
        }
    }
}
