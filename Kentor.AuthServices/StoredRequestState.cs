using System;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Stored data for each PendingAuthnRequest
    /// </summary>
    [Serializable]
    public class StoredRequestState
    {
        /// <summary>
        /// Creates a PendingAuthnRequestData
        /// </summary>
        /// <param name="id">The identifier of the request.</param>
        /// <param name="idpEntityId">The EntityId of the IDP the request was sent to</param>
        /// <param name="returnUrl">The Url to redirect back to after a succesful login</param>
        public StoredRequestState(string id, string idpEntityId, Uri returnUrl)
        {
            Id = id; 
            IdpEntityId = idpEntityId;
            ReturnUrl = returnUrl;
        }

        /// <summary>
        /// Creates a PendingAuthnRequestData
        /// </summary>
        /// <param name="id">The identifier of the request.</param>
        /// <param name="idpEntityId">The EntityId of the IDP the request was sent to</param>
        /// <param name="returnUrl">The Url to redirect back to after a succesful login</param>
        /// <param name="relayData">Aux data that can be stored across the authentication request.</param>
        public StoredRequestState(string id, string idpEntityId, Uri returnUrl, object relayData)
        {
            Id = id;
            IdpEntityId = idpEntityId;
            ReturnUrl = returnUrl;
            RelayData = relayData;
        }
        
        /// <summary>
        /// The ID of the authentication request.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The IDP the request was sent to
        /// </summary>
        public string IdpEntityId { get; private set; }

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
