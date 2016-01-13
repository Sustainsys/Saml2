namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Signing behavior for requests.
    /// </summary>
    public enum SigningBehavior
    {
        /// <summary>
        /// Never sign AuthnRequests. This is the default behavior.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Always sign AuthnRequests. AuthnRequestsSigned is set to true
        /// in metadata.
        /// </summary>
        Always = 1,

        // Possible future enhancement.
        // IfIdpWantAuthnRequestsSigned = 3,
    }
}