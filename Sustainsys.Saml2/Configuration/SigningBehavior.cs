namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Signing behavior for requests.
    /// </summary>
    public enum SigningBehavior
    {
        /// <summary>
        /// Sign authnrequests if the idp is configured for it. This is the 
        /// default behavior.
        /// </summary>
        IfIdpWantAuthnRequestsSigned = 0,

        /// <summary>
        /// Always sign AuthnRequests. AuthnRequestsSigned is set to true
        /// in metadata.
        /// </summary>
        Always = 1,

        /// <summary>
        /// Never sign AuthnRequests.
        /// </summary>
        Never = 3,
    }
}