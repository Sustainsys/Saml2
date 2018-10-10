using Sustainsys.Saml2.WebSso;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Net;
using System.Web;
using System.Web.Security;

namespace Sustainsys.Saml2.HttpModule
{
    /// <summary>
    /// Extension methods to CommandResult to update a HttpResponseBase.
    /// </summary>
    public static partial class CommandResultHttpExtensions
    {
        /// <summary>
        /// Apply cookies of the CommandResult to the response.
        /// </summary>
        /// <param name="commandResult">Commandresult</param>
        /// <param name="response">Response</param>
        public static void ApplyCookies(this CommandResult commandResult, HttpResponseBase response)
        {
            if(commandResult == null)
            {
                throw new ArgumentNullException(nameof(commandResult));
            }

            if(response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (!string.IsNullOrEmpty(commandResult.SetCookieName))
            {
                var protectedData = HttpRequestData.ConvertBinaryData(
                        MachineKey.Protect(
                            commandResult.GetSerializedRequestState(),
                            HttpRequestBaseExtensions.ProtectionPurpose));

                response.SetCookie(new HttpCookie(
                    commandResult.SetCookieName,
                    protectedData)
                {
                    HttpOnly = true
                });
            }

            if (!string.IsNullOrEmpty(commandResult.ClearCookieName))
            {
                response.SetCookie(new HttpCookie(commandResult.ClearCookieName)
                {
                    Expires = new DateTime(1970, 01, 01)
                });
            }
        }

        /// <summary>
        /// Apply headers of the command result to the response.
        /// </summary>
        /// <param name="commandResult">Command result containing headers.</param>
        /// <param name="response">Response to set headers in.</param>
        public static void ApplyHeaders(this CommandResult commandResult, HttpResponseBase response)
        {
            if(commandResult == null)
            {
                throw new ArgumentNullException(nameof(commandResult));
            }

            if(response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            foreach (var h in commandResult.Headers)
            {
                response.AddHeader(h.Key, h.Value);
            }
        }

        /// <summary>
        /// Establishes an application session by calling the session authentication module.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static void SignInOrOutSessionAuthenticationModule(this CommandResult commandResult)
        {
            if (commandResult == null)
            {
                throw new ArgumentNullException(nameof(commandResult));
            }

            // Ignore this if we're not running inside IIS, e.g. in unit tests.
            if (commandResult.Principal != null && HttpContext.Current != null)
            {
                var sessionToken = new SessionSecurityToken(
                    commandResult.Principal,
                    null,
                    DateTime.UtcNow,
                    commandResult.SessionNotOnOrAfter ??
                    CalculateSessionNotOnOrAfter());

                EnsureSessionAuthenticationModuleAvailable();

                FederatedAuthentication.SessionAuthenticationModule
                    .AuthenticateSessionSecurityToken(sessionToken, true);
            }
            if (commandResult.TerminateLocalSession && HttpContext.Current != null)
            {
                EnsureSessionAuthenticationModuleAvailable();

                FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
            }
        }

        [ExcludeFromCodeCoverage]
        private static DateTime CalculateSessionNotOnOrAfter()
        {
            var configuredLifeTime = (FederatedAuthentication.FederationConfiguration
                    .IdentityConfiguration.SecurityTokenHandlers[typeof(SessionSecurityToken)]
                    as SessionSecurityTokenHandler).TokenLifetime;

            return DateTime.UtcNow.Add(configuredLifeTime);
        }

        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "Several words in the error message")]
        private static void EnsureSessionAuthenticationModuleAvailable()
        {
            if (FederatedAuthentication.SessionAuthenticationModule == null)
            {
                throw new InvalidOperationException(
                    "FederatedAuthentication.SessionAuthenticationModule is null, make sure you have loaded the SessionAuthenticationModule in web.config.");
            }
        }
    }
}
