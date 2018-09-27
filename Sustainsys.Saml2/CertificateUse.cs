namespace Sustainsys.Saml2
{
    /// <summary>
    /// How is the certificate used?
    /// </summary>
    public enum CertificateUse
    {
        /// <summary>
        /// The certificate is used for either signing or encryption, or both
        /// </summary>
        Both = 0,

        /// <summary>
        /// The certificate is used for signing outbound requests
        /// </summary>
        Signing = 1,

        /// <summary>
        /// The certificate is used for decrypting inbound assertions
        /// </summary>
        Encryption = 2
    }
}
