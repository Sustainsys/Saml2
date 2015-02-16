using System;
using System.IdentityModel.Metadata;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Stored data for each PendingAuthnRequest
    /// </summary>
    public class StoredRequestState
    {
        /// <summary>
        /// Creates a StoredRequestState
        /// </summary>
        /// <param name="idp">The EntityId of the IDP the request was sent to</param>
        /// <param name="returnUrl">The Url to redirect back to after a succesful login</param>
        public StoredRequestState(EntityId idp, Uri returnUrl)
        {
            Idp = idp;
            ReturnUrl = returnUrl;
        }

        /// <summary>
        /// Creates a StoredRequestState
        /// </summary>
        /// <param name="idp">The EntityId of the IDP the request was sent to</param>
        /// <param name="returnUrl">The Url to redirect back to after a succesful login</param>
        /// <param name="relayData">Aux data that can be stored across the authentication request.</param>
        public StoredRequestState(EntityId idp, Uri returnUrl, object relayData)
        {
            Idp = idp;
            ReturnUrl = returnUrl;
            RelayData = relayData;
        }

        /// <summary>
        /// The IDP the request was sent to
        /// </summary>
        public EntityId Idp { get; private set; }

        /// <summary>
        /// The Url to redirect back to after a succesful login
        /// </summary>
        public Uri ReturnUrl { get; private set; }

        /// <summary>
        /// Aux data that need to be preserved across the authentication call.
        /// </summary>
        public object RelayData { get; private set; }
    }
}
