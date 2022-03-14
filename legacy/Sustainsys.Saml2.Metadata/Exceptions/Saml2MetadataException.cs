using System;
using System.Runtime.Serialization;

namespace Sustainsys.Saml2.Metadata.Exceptions
{
    /// <summary>
    /// Base class for metadata specific exceptions, that might
    /// require special handling for error reporting to the user.
    /// </summary>
    [Serializable]
    public abstract class Saml2MetadataException : Exception
    {
        /// <summary>
        /// Default Ctor
        /// </summary>
        protected Saml2MetadataException() { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        protected Saml2MetadataException(string message)
            : base(message)
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception.</param>
        protected Saml2MetadataException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Serialization Ctor
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected Saml2MetadataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}