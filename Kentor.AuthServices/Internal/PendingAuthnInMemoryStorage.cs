using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Internal
{
    /// <summary>
    /// Manages storing and retrieving Pending Authentication Data while the authentication handshake is in progress.
    /// </summary>
    public class PendingAuthnInMemoryStorage : Kentor.AuthServices.IPendingAuthStorageContainer
    {
        private static Dictionary<Saml2Id, StoredRequestState> container = new Dictionary<Saml2Id, StoredRequestState>();

        /// <summary>
        /// Stores the StoredRequestState using the Saml2Id
        /// </summary>
        /// <param name="id">The Saml2Id used as key to store the pending authentication data.</param>
        /// <param name="idp">The state of the pending authentication request.</param>
        public void Add(Saml2Id id, StoredRequestState idp)
        {
            lock (container)
            {
                if (container.ContainsKey(id))
                {
                    throw new InvalidOperationException("AuthnRequest id can't be reused.");
                }
                container.Add(id, idp);
            }
        }

        /// <summary>
        /// Returns the Stored Request State while simultaneously deleting it from storage. 
        /// </summary>
        /// <param name="id">The Saml2Id key used to retrieve the pending authentication data.</param>
        /// <param name="idp">The state of the pending authentication request.</param>
        /// <returns></returns>
        public bool TryRemove(Saml2Id id, out StoredRequestState idp)
        {
            lock (container)
            {
                if (id != null && container.ContainsKey(id))
                {
                    idp = container[id];
                    return container.Remove(id);
                }
                idp = null;
                return false;
            }
        }
    }
}