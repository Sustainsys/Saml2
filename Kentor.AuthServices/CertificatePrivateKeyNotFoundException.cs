using System;
using System.Globalization;
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
        /// Ctor
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public CertificatePrivateKeyNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public CertificatePrivateKeyNotFoundException(string message)
            : base(message)
        { }

        /// <summary>
        /// Default Ctor, setting message to a default.
        /// </summary>
        /// <param name="certificate">The certificate that the private key was not found or access was denied.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public CertificatePrivateKeyNotFoundException(X509Certificate2 certificate)
            : this(certificate, String.Format(CultureInfo.InvariantCulture, "The private key for the certificate '{0}' was not found or access was denied.", certificate.SubjectName))
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
            if (certificate == null) 
            {
                throw new ArgumentNullException("certificate");
            }

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

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param><param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination. </param><exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). </exception><filterpriority>2</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/></PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            if (info == null) {
                throw new ArgumentNullException("info");
            }
                
            info.AddValue("Certificate", Certificate);
        }
    }
}
