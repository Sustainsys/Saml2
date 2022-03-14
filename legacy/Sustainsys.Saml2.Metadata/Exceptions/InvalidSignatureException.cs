using System;
using System.Runtime.Serialization;

namespace Sustainsys.Saml2.Metadata.Exceptions
{
    /// <summary>
    /// Exception thrown when an signature is not valid according to the
    /// SAML standard.
    /// </summary>
    [Serializable]
    public class InvalidSignatureException : Saml2MetadataException
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public InvalidSignatureException() { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message of exception</param>
        public InvalidSignatureException(string message)
            : base(message)
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
        public InvalidSignatureException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Serialization Ctor
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected InvalidSignatureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}