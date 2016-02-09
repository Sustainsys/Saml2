namespace Kentor.AuthServices
{
    /// <summary>
    /// Claim type constants.
    /// </summary>
    public static class AuthServicesClaimTypes
    {
        internal const string ClaimTypeNamespace = "http://kentor.se/AuthServices";

        /// <summary>
        /// Session index is set by the idp and is used to correlate sessions
        /// during single logout.
        /// </summary>
        public const string SessionIndex = ClaimTypeNamespace + "/SessionIndex";
    }
}
