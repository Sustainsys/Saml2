﻿using System;
using System.Runtime.Serialization;

namespace Kentor.AuthServices
{
    /// <summary>
    /// No saml response was found in the http request.
    /// </summary>
    [Serializable]
    public class CertificateNotFoundException : AuthServicesException
    {
        /// <summary>
        /// Default Ctor, setting message to a default.
        /// </summary>
        public CertificateNotFoundException()
            : this("Certificate not found.")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public CertificateNotFoundException(string message)
            : base(message)
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public CertificateNotFoundException(string message, Exception innerException)
            :base(message, innerException)
        { }

        /// <summary>
        /// Serialization Ctor
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected CertificateNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
