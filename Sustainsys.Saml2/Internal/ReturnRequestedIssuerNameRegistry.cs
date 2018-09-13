using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using Sustainsys.Saml2.Configuration;

namespace Sustainsys.Saml2.Internal
{
    /// <summary>
    /// A dummy IssuerNameRegistry implementation that just returns whatever issuer name
    /// was requested. It is intended to use ONLY in cases where the issuer already have
    /// been verified by other means (such as a signature on the entire response message)
    /// </summary>
    class ReturnRequestedIssuerNameRegistry : IssuerNameRegistry
    {
        public override string GetIssuerName(SecurityToken securityToken, string requestedIssuerName)
        {
            return requestedIssuerName;
        }

        public override string GetIssuerName(SecurityToken securityToken)
        {
            throw new NotImplementedException();
        }
    }
}
