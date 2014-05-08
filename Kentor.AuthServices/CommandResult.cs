using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace Kentor.AuthServices
{
    using System.Collections.Generic;
    using System.IdentityModel.Services.Configuration;
    using System.Linq;

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

        /// <summary>The extracted security tokens if the command has parsed an incoming assertion.</summary>
        /// <value>The security tokens.</value>
        public IEnumerable<SecurityToken> SecurityTokens { get; set; }

        /// <summary>
        /// The response body that is the result of the command.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public CommandResult()
        {
            HttpStatusCode = HttpStatusCode.OK;
            Cacheability = HttpCacheability.NoCache;
            SecurityTokens = new List<SecurityToken>();
        }

        /// <summary>
        /// Apply the command result to a bare HttpResponse.
        /// </summary>
        /// <param name="response">Http Response to write the result to.</param>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "HttpStatusCode")]
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

                response.Redirect(Location.ToString());
            }

            response.StatusCode = (int)HttpStatusCode;
        }

        internal virtual SessionSecurityToken CreateSessionSecurityToken()
        {
            // We should respect the validity of the SecurityTokens to set the session lifetime
            // If there are multiple security tokens (assertions) with different validity we calculate the shortest timespan as the token lifetime
            var validForm = SecurityTokens.Max(t => t.ValidFrom).ToUniversalTime();
            var validTo = SecurityTokens.Min(t => t.ValidTo).ToUniversalTime();
            
            var securityTokenHandler = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers[typeof(SessionSecurityToken)] as SessionSecurityTokenHandler;

            if (validForm == DateTime.MinValue.ToUniversalTime())
            {
                validForm = DateTime.UtcNow;
            }

            if (validTo == DateTime.MaxValue.ToUniversalTime())
            {
                validTo = validForm.Add(SessionSecurityTokenHandler.DefaultTokenLifetime);
            }
            
            // This is just a workarround for situations where no HttpContext is available (tests)
            // FederatedAuthentication.SessionAuthenticationModule.CreateSessionSecurityToken needs a HttpContext
            if (HttpContext.Current == null)
            {
                var sessionSecurityToken = securityTokenHandler.CreateSessionSecurityToken(Principal, null, string.Empty, validForm, validTo);
                sessionSecurityToken.IsPersistent = false;
                sessionSecurityToken.IsReferenceMode = false;
                return sessionSecurityToken;
            }

            // It's better to use CreateSessionSecurityToken of SessionAuthenticationModule because this does 
            return FederatedAuthentication.SessionAuthenticationModule.CreateSessionSecurityToken(Principal, null, validForm, validTo, false);
        }

        /// <summary>
        /// Applies the principal found in the command result by a call to the 
        /// session auth module.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void ApplyPrincipal()
        {
            // Ignore this if we're not running inside IIS, e.g. in unit tests.
            if(Principal != null && HttpContext.Current != null)
            {
                var sessionToken = this.CreateSessionSecurityToken();
                FederatedAuthentication.SessionAuthenticationModule
                    .AuthenticateSessionSecurityToken(sessionToken, true);
            }
        }
    }
}
