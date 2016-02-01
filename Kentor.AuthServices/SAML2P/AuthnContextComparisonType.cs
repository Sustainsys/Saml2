namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// Comparison setting for RequestedAuthnContext, see SAML2 Core spec 3.3.2.2.1.
    /// </summary>
    public enum AuthnContextComparisonType
    {
        /// <summary>
        /// Exact match is required. This is the default.
        /// </summary>
        Exact = 0,

        /// <summary>
        /// The resulting AuthnContext must be at least as strong as the
        /// specified classRef.
        /// </summary>
        Minimum = 1,

        /// <summary>
        /// The resulting AuthnContext must be at most as strong as the
        /// specified classRef.
        /// </summary>
        Maximum = 2,

        /// <summary>
        /// The resulting AuthnContext must be better than the specified
        /// classRef. The classRef specified is thus not permitted.
        /// </summary>
        Better = 3
    }
}
