using System;

namespace Sustainsys.Saml2
{
    /// <summary>
    /// How is the certificate used?
    /// </summary>
    [Flags]
    public enum CertificateUse
    {
        /// <summary>
        /// The certificate is used for either signing or encryption, or both.
        /// Equivalent to Signing | Encryption.
        /// </summary>
        Both = 0,

        /// <summary>
        /// The certificate is used for signing outbound requests
        /// </summary>
        Signing = 1,

        /// <summary>
        /// The certificate is used for decrypting inbound assertions
        /// </summary>
        Encryption = 2,

        /// <summary>
        /// The certificate is used as a Tls Client certificate for outbound
        /// tls requests.
        /// </summary>
        TlsClient = 4
    }
}
