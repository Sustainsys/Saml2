using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Internal
{
    /// <summary>
    /// In memory container for storing in-progress Authentication requests. 
    /// </summary>
    internal static class PendingAuthnInMemoryContainer
    {
        /// <summary>
        /// In memory dictionary for keeping track of pending authentications. Only for use with single-server deployments, not in a web farm. 
        /// </summary>
        internal static Dictionary<Saml2Id, StoredRequestState> Container = new Dictionary<Saml2Id, StoredRequestState>();
    }
}
