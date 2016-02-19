using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
namespace Kentor.AuthServices.Internal
{
    static class PendingAuthnRequests
    {
        private static MemoryCache pendingRequestCache = MemoryCache.Default;
        //private static readonly Dictionary<Saml2Id, StoredRequestState> pendingAuthnRequest = new Dictionary<Saml2Id, StoredRequestState>();

        internal static void Add(Saml2Id id, StoredRequestState idp)
        {
            lock (pendingRequestCache)
            {
                if (pendingRequestCache.Contains(id.Value))
                {
                    throw new InvalidOperationException("AuthnRequest id can't be reused.");
                }
                pendingRequestCache.Add(id.Value, idp, new CacheItemPolicy { });
            }
        }

        internal static bool TryRemove(Saml2Id id, out StoredRequestState idp)
        {
            lock (pendingRequestCache)
            {
                if (id != null && pendingRequestCache.Contains(id.Value))
                {
                    idp = pendingRequestCache[id.Value] as StoredRequestState;
                    pendingRequestCache.Remove(id.Value);
                    return true;
                }
                idp = null;
                return false;
            }
        }
    }
}
