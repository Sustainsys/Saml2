using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Stored data for each PendingAuthnRequest
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "AuthnRequest")] // AuthnRequest is already in dictionary
    public class StoredRequestState
    {
        /// <summary>
        /// Creates a PendingAuthnRequestData
        /// </summary>
        /// <param name="idp">The EntityId of the IDP the request was sent to</param>
        /// <param name="returnUrl">The Url to redirect back to after a succesful login</param>
        public StoredRequestState(EntityId idp, Uri returnUrl)
        {
            Idp = idp;
            ReturnUrl = returnUrl;
        }

        /// <summary>
        /// The IDP the request was sent to
        /// </summary>
        public EntityId Idp { get; private set; }

        /// <summary>
        /// The Url to redirect back to after a succesful login
        /// </summary>
        public Uri ReturnUrl { get; private set; }
    }
}
