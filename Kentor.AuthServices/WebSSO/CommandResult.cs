using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace Kentor.AuthServices.WebSso
{
    /// <summary>
    /// The results of a command.
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// Status code that should be returned.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }
        
        /// <summary>
        /// Cacheability of the command result.
        /// </summary>
        public HttpCacheability Cacheability { get; set; }
        
        /// <summary>
        /// Location, if the status code is a redirect.
        /// </summary>
        public Uri Location { get; set; }
        
        /// <summary>
        /// The extracted principal if the command has parsed an incoming assertion.
        /// </summary>
        public ClaimsPrincipal Principal { get; set; }

        /// <summary>
        /// The response body that is the result of the command.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The Mime-type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public CommandResult()
        {
            HttpStatusCode = HttpStatusCode.OK;
            Cacheability = HttpCacheability.NoCache;
        }

        /// <summary>
        /// Apply the command result to a bare HttpResponse.
        /// </summary>
        /// <param name="response">Http Response to write the result to.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "HttpStatusCode")]
        public void Apply(HttpResponseBase response)
        {
            if(response == null)
            {
                throw new ArgumentNullException("response");
            }

            response.Cache.SetCacheability(Cacheability);

            if (HttpStatusCode == HttpStatusCode.SeeOther || Location != null)
            {
                if (Location == null)
                {
                    throw new InvalidOperationException("Missing Location on redirect.");
                }
                if (HttpStatusCode != HttpStatusCode.SeeOther)
                {
                    throw new InvalidOperationException("Invalid HttpStatusCode for redirect, but Location is specified");
                }

                response.Redirect(Location.OriginalString);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode;
                response.ContentType = ContentType;
                response.Write(Content);

                response.End();
            }
        }

        /// <summary>
        /// Establishes an application session by calling the session authentication module.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void SignInSessionAuthenticationModule()
        {
            // Ignore this if we're not running inside IIS, e.g. in unit tests.
            if(Principal != null && HttpContext.Current != null)
            {
                var handler = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers[typeof(SessionSecurityToken)] as SessionSecurityTokenHandler;

                var lifetime = (handler != null) ? handler.TokenLifetime : SessionSecurityTokenHandler.DefaultTokenLifetime;

                var sessionToken = new SessionSecurityToken(Principal, lifetime);

                FederatedAuthentication.SessionAuthenticationModule
                    .AuthenticateSessionSecurityToken(sessionToken, true);
            }
        }
    }
}
