namespace Kentor.AuthServices
{
    /// <summary>
    /// Is this certificate for current or future use?
    /// </summary>
    public enum CertificateStatus
    {
        /// <summary>
        /// The certificate is used for current requests
        /// </summary>
        Current = 0,

        /// <summary>
        /// The certificate is used for current and/or future requests
        /// </summary>
        Future = 1
    }
}
