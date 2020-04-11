using System;
using System.Runtime.Serialization;

namespace Sustainsys.Saml2.Metadata.Exceptions
{
    /// <summary>
    /// Exception thrown when metadata is not valid according to the
    /// SAML standard.
    /// </summary>
    [Serializable]
    public class MetadataSerializationException : Exception
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public MetadataSerializationException() { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message of exception</param>
        public MetadataSerializationException(string message)
            : base(message)
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
        public MetadataSerializationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Serialization Ctor
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected MetadataSerializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}