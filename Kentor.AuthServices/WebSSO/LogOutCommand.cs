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

            var returnUrl = request.QueryString["ReturnUrl"].SingleOrDefault();

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
            var binding = options.Notifications.GetBinding(request);
            if (binding != null)
            {
                var unbindResult = binding.Unbind(request, options);
                VerifyMessageIsSigned(unbindResult, options);
                switch (unbindResult.Data.LocalName)
                {
                    case "LogoutRequest":
                        commandResult = HandleRequest(unbindResult, options);
                        break;
                    case "LogoutResponse":
                        commandResult = HandleResponse(unbindResult, request);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                commandResult = InitiateLogout(request, returnPath, options);
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

                if (!unbindResult.Data.IsSignedByAny(idp.SigningKeys, options.SPOptions.ValidateCertificates))
                {
                    throw new UnsuccessfulSamlOperationException(string.Format(CultureInfo.InvariantCulture,
                        "Received a {0} from {1} that cannot be processed because it is not signed.",
                        unbindResult.Data.LocalName,
                        unbindResult.Data["Issuer", Saml2Namespaces.Saml2Name].InnerText));
                }
            }
        }

        private static CommandResult InitiateLogout(HttpRequestData request, string returnPath, IOptions options)
        {
            string idpEntityId = null;
            Claim sessionIndexClaim = null;
            if (request.User != null)
            {
                idpEntityId = request.User.FindFirst(AuthServicesClaimTypes.LogoutNameIdentifier)?.Issuer;
                sessionIndexClaim = request.User.FindFirst(AuthServicesClaimTypes.SessionIndex);
            }

            CommandResult commandResult;
            IdentityProvider idp;
            if(idpEntityId != null 
                && options.IdentityProviders.TryGetValue(new EntityId(idpEntityId), out idp)
                && sessionIndexClaim != null
                && idp.SingleLogoutServiceUrl != null
                && options.SPOptions.SigningServiceCertificate != null
                && !idp.DisableOutboundLogoutRequests)
            {
                var logoutRequest = idp.CreateLogoutRequest(request.User);

                commandResult = Saml2Binding.Get(idp.SingleLogoutServiceBinding)
                    .Bind(logoutRequest);

                commandResult.RequestState = new StoredRequestState(
                    idp.EntityId,
                    GetReturnUrl(request, returnPath, options),
                    logoutRequest.Id,
                    null);

                commandResult.SetCookieName = "Kentor." + logoutRequest.RelayState;
            }
            else
            {
                commandResult = new CommandResult
                {
                    HttpStatusCode = HttpStatusCode.SeeOther,
                    Location = GetReturnUrl(request, returnPath, options)
                };
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
                return new AuthServicesUrls(request, options.SPOptions).ApplicationUrl;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "LogoutRequest")]
        private static CommandResult HandleRequest(UnbindResult unbindResult, IOptions options)
        {
            var request = Saml2LogoutRequest.FromXml(unbindResult.Data);

            var idp = options.IdentityProviders[request.Issuer];

            if(options.SPOptions.SigningServiceCertificate == null)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture,
                    "Received a LogoutRequest from \"{0}\" but cannot reply because single logout responses must be signed and there is no signing certificate configured. Looks like the idp is configured for Single Logout despite AuthServices not exposing that functionality in the metadata.",
                    request.Issuer.Id));
            }

            var response = new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = idp.SingleLogoutServiceResponseUrl,
                SigningCertificate = options.SPOptions.SigningServiceCertificate,
                InResponseTo = request.Id,
                Issuer = options.SPOptions.EntityId,
                RelayState = unbindResult.RelayState
            };

            var result = Saml2Binding.Get(idp.SingleLogoutServiceBinding).Bind(response);
            result.TerminateLocalSession = true;
            return result;
        }

        private static CommandResult HandleResponse(UnbindResult unbindResult, HttpRequestData request)
        {
            var status = Saml2LogoutResponse.FromXml(unbindResult.Data).Status;
            if(status != Saml2StatusCode.Success)
            {
                throw new UnsuccessfulSamlOperationException(string.Format(CultureInfo.InvariantCulture,
                    "Idp returned status \"{0}\", indicating that the single logout failed. The local session has been successfully terminated.",
                    status));
            }

            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = request.StoredRequestState.ReturnUrl,
                ClearCookieName = "Kentor." + unbindResult.RelayState
            };
        }
    }
}
