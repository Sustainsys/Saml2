namespace Kentor.AuthServices.AspNetCore
{
    /// <summary>
    /// Default values used by KentorAuthServices authentication.
    /// </summary>
    public static class KentorAuthServicesDefaults
    {
        /// <summary>
        /// Default value for AuthenticationScheme property in the KentorAuthServicesOptions
        /// </summary>
        public const string DefaultAuthenticationScheme = "KentorAuthServices";
        /// <summary>
        /// Default value for DisplayName property in the KentorAuthServicesOptions
        /// </summary>
        public const string DefaultDisplayName = "SAML2 Federation";
    }
}
