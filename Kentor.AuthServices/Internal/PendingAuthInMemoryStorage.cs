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
    public class PendingAuthInMemoryStorage : Kentor.AuthServices.IPendingAuthStorageContainer
    {
        /// <summary>
        /// Stores the StoredRequestState using the Saml2Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idp"></param>
        public void Add(Saml2Id id, StoredRequestState idp)
        {
            lock (PendingAuthnInMemoryContainer.Container)
            {
                if (PendingAuthnInMemoryContainer.Container.ContainsKey(id))
                {
                    throw new InvalidOperationException("AuthnRequest id can't be reused.");
                }
                PendingAuthnInMemoryContainer.Container.Add(id, idp);
            }
        }

        /// <summary>
        /// Returns the Stored Request State while simultaneously deleting it from storage. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idp"></param>
        /// <returns></returns>
        public bool TryRemove(Saml2Id id, out StoredRequestState idp)
        {
            lock (PendingAuthnInMemoryContainer.Container)
            {
                if (id != null && PendingAuthnInMemoryContainer.Container.ContainsKey(id))
                {
                    idp = PendingAuthnInMemoryContainer.Container[id];
                    return PendingAuthnInMemoryContainer.Container.Remove(id);
                }
                idp = null;
                return false;
            }
        }
    }
}