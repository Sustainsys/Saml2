using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace Kentor.AuthServices
{
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

        /// <summary>The Saml2Response if the command has parsed an incoming assertion.</summary>
        /// <value>The saml2 response.</value>
        public Saml2Response Saml2Response { get; set; }

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
        
        /// <summary>
        /// Applies the principal found in the command result by a call to the 
        /// session auth module.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void ApplyPrincipal()
        {
            // Ignore this if we're not running inside IIS, e.g. in unit tests.
            if (Principal != null && HttpContext.Current != null)
            {
                var sessionToken = this.CreateSessionSecurityToken();
                FederatedAuthentication.SessionAuthenticationModule
                    .AuthenticateSessionSecurityToken(sessionToken, true);
            }
        }

        internal struct Validity
        {
            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }

        internal Validity ComputeTokenValidity()
        {
            // Interesting articles abot session lifetim in WIF:
            // http://brockallen.com/2013/02/14/configuring-session-token-lifetime-in-wif-with-the-session-authentication-module-sam-and-thinktecture-identitymodel/
            // http://www.cloudidentity.com/blog/2010/06/16/warning-sliding-sessions-are-closer-than-they-appear/
            // http://www.cloudidentity.com/blog/2013/05/08/sliding-sessions-for-wif-4-5/
            // This computation is based on information of these articles (and even more)

            var securityTokenHandler = GetCurrentSessionSecurityTokenHandler();
            var configuredSessionTokenLifetime = securityTokenHandler.TokenLifetime;

            // We should respect the validity of the SecurityTokens to set the session lifetime
            // If there are multiple security tokens (assertions) with different validity we calculate the shortest timespan as the token lifetime
            var tokenValidFrom = Saml2Response.Saml2SecurityTokens.Max(t => t.ValidFrom).ToUniversalTime();
            var tokenValidTo = Saml2Response.Saml2SecurityTokens.Min(t => t.ValidTo).ToUniversalTime();
            
            var defaultValidFrom = DateTime.UtcNow;
            var defaultValidTo = defaultValidFrom.Add(configuredSessionTokenLifetime);

            if (tokenValidFrom == DateTime.MinValue.ToUniversalTime())
            {
                tokenValidFrom = Saml2Response.IssueInstant.ToUniversalTime();
            }

            if (tokenValidTo == DateTime.MaxValue.ToUniversalTime())
            {
                tokenValidTo = tokenValidFrom.Add(configuredSessionTokenLifetime);
            }

            // If the RP restricts the token lifetime to a shorter value, we take the restricted lifetime
            var tokenLifetime = tokenValidTo.Subtract(tokenValidFrom);
            if (configuredSessionTokenLifetime < tokenLifetime)
            {
                return new Validity { From = defaultValidFrom, To = defaultValidTo };
            }

            return new Validity { From = tokenValidFrom, To = tokenValidTo };
        }

        private static SessionSecurityTokenHandler GetCurrentSessionSecurityTokenHandler()
        {
            return FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers[typeof(SessionSecurityToken)] as SessionSecurityTokenHandler;
        }

        [ExcludeFromCodeCoverage]
        internal virtual SessionSecurityToken CreateSessionSecurityToken()
        {
            var validity = this.ComputeTokenValidity();

            // This is just a workarround for situations where no HttpContext is available (tests)
            // FederatedAuthentication.SessionAuthenticationModule.CreateSessionSecurityToken needs a HttpContext
            if (HttpContext.Current == null)
            {
                var securityTokenHandler = GetCurrentSessionSecurityTokenHandler();
                var sessionSecurityToken = securityTokenHandler.CreateSessionSecurityToken(Principal, null, string.Empty, validity.From, validity.To);
                sessionSecurityToken.IsPersistent = false;
                sessionSecurityToken.IsReferenceMode = false;

                return sessionSecurityToken;
            }

            // It's better to use CreateSessionSecurityToken of SessionAuthenticationModule because it uses the configured CookieHandler and othe configuration values
            return FederatedAuthentication.SessionAuthenticationModule.CreateSessionSecurityToken(Principal, null, validity.From, validity.To, false);
        }
    }
}
