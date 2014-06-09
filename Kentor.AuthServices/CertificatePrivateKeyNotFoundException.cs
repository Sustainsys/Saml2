using System;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace Kentor.AuthServices
{
    /// <summary>
    /// No saml response was found in the http request.
    /// </summary>
    [Serializable]
    public class CertificatePrivateKeyNotFoundException : AuthServicesException
    {
        /// <summary>
        /// The certificate that the private key was not found or access was denied.
        /// </summary>
        public X509Certificate2 Certificate { get; private set; }

        /// <summary>
        /// Default Ctor, setting message to a default.
        /// </summary>
        public CertificatePrivateKeyNotFoundException()
            : base("The private key for the certificate was not found or access was denied.")
        { }
        
        /// <summary>
        /// Default Ctor, setting message to a default.
        /// </summary>
        /// <param name="certificate">The certificate that the private key was not found or access was denied.</param>
        public CertificatePrivateKeyNotFoundException(X509Certificate2 certificate)
            : this(certificate, String.Format("The private key for the certificate '{0}' was not found or access was denied.", certificate.SubjectName))
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="certificate">The certificate that the private key was not found or access was denied.</param>
        /// <param name="message">Message of the exception.</param>
        public CertificatePrivateKeyNotFoundException(X509Certificate2 certificate, string message)
            : this(certificate, message, null)
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="certificate">The certificate that the private key was not found or access was denied.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public CertificatePrivateKeyNotFoundException(X509Certificate2 certificate, string message, Exception innerException)
            : base(message, innerException)
        {
            Certificate = certificate;
        }

        /// <summary>
        /// Serialization Ctor
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected CertificatePrivateKeyNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
