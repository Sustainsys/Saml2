using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Invalid SAML metadata was encountered.
    /// </summary>
    [Serializable]
    public class InvalidMetadataException : AuthServicesException
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public InvalidMetadataException()
        { }

        /// <summary>
        /// Creates an InvalidMetadataException with the given message.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public InvalidMetadataException(string message)
            : base(message)
        { }

        /// <summary>
        /// Serialization Ctor
        /// </summary>
        /// <param name="info">Serialization Infor</param>
        /// <param name="context">Streaming Context</param>
        protected InvalidMetadataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public InvalidMetadataException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
