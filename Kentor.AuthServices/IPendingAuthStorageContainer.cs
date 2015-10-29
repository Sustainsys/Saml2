using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Storage container for authentication state that must be present when an authentication response is received back from the Identity Provider.
    /// </summary>
    public interface IPendingAuthStorageContainer
    {
        /// <summary>
        /// Add state that must be available after external authentication response comes back from IdP. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idp"></param>
        void Add(System.IdentityModel.Tokens.Saml2Id id, Kentor.AuthServices.StoredRequestState idp);

        /// <summary>
        /// Retrieve and delete the stored authentication state that is needed to complete an external authentication. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idp"></param>
        /// <returns></returns>
        bool TryRemove(System.IdentityModel.Tokens.Saml2Id id, out Kentor.AuthServices.StoredRequestState idp);
    }
}
