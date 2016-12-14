namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// Enum MessageSigningAlgorithm
    /// </summary>
    public static class MessageSigningAlgorithm
    {
        /// <summary>
        /// The rsasha1
        /// </summary>
        public const string RsaSecureHashAlgorithm1 = "RsaSecureHashAlgorithm1";
        /// <summary>
        /// The rsasha256
        /// </summary>
        public const string RsaSecureHashAlgorithm256 = "RsaSecureHashAlgorithm256";
        /// <summary>
        /// The rsasha384
        /// </summary>
        public const string RsaSecureHashAlgorithm384 = "RsaSecureHashAlgorithm384";
        /// <summary>
        /// The rsasha512
        /// </summary>
        public const string RsaSecureHashAlgorithm512 = "RsaSecureHashAlgorithm512";

    }

    /// <summary>
    /// Class MessageSigningDefaults.
    /// </summary>
    public static class MessageSigningDefaults
    {
        /// <summary>
        /// The default algorithm
        /// </summary>
        public const string DefaultAlgorithm =
            MessageSigningAlgorithm.RsaSecureHashAlgorithm1;
    }
}