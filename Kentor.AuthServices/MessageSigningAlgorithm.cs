namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// Enum MessageSigningAlgorithm
    /// </summary>
    public enum MessageSigningAlgorithm
    {
        /// <summary>
        /// The rsasha1
        /// </summary>
        RsaSecureHashAlgorithm1,
        /// <summary>
        /// The rsasha256
        /// </summary>
        RsaSecureHashAlgorithm256,
        /// <summary>
        /// The rsasha384
        /// </summary>
        RsaSecureHashAlgorithm384,
        /// <summary>
        /// The rsasha512
        /// </summary>
        RsaSecureHashAlgorithm512

    }

    /// <summary>
    /// Class MessageSigningDefaults.
    /// </summary>
    public static class MessageSigningDefaults
    {
        /// <summary>
        /// The default algorithm
        /// </summary>
        public const MessageSigningAlgorithm DefaultAlgorithm =
            MessageSigningAlgorithm.RsaSecureHashAlgorithm1;
    }
}