using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Internal
{
    static class PendingAuthnRequests
    {
        private static readonly ConcurrentDictionary<string, StoredRequestState> pendingAuthnRequest
            = new ConcurrentDictionary<string, StoredRequestState>();

        internal static void Add(string id, StoredRequestState state)
        {
            ((IDictionary<string, StoredRequestState>)pendingAuthnRequest).Add(id, state);
        }

        internal static bool TryRemove(string id, out StoredRequestState state)
        {
            state = null;
            return pendingAuthnRequest.TryRemove(id, out state);
        }

        internal static StoredRequestState Get(string id)
        {
            return pendingAuthnRequest[id];
        }
    }
}
