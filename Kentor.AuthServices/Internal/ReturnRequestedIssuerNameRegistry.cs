using System;
using System.IdentityModel.Tokens;
using System.Linq;

namespace Kentor.AuthServices.Internal
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
