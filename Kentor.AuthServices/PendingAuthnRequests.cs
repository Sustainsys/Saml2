using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    static class PendingAuthnRequests
    {
        private static readonly HashSet<Saml2Id> pendingAuthnRequest = new HashSet<Saml2Id>();

        internal static void Add(Saml2Id id)
        {
            lock (pendingAuthnRequest)
            {
                if (!pendingAuthnRequest.Add(id))
                {
                    throw new InvalidOperationException("AuthnRequest id can't be reused.");
                }
            }
        }

        internal static void Remove(Saml2Id id)
        {
            lock (pendingAuthnRequest)
            {
                if (!pendingAuthnRequest.Remove(id))
                {
                    throw new InvalidOperationException("AuthnRequest id was never issued or has already been used.");
                }
            }
        }
    }
}
