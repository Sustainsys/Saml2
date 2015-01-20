using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Saml2 Metadata object has failed validation.
    /// </summary>
    [Serializable]
    public class MetadataFailedValidationException : AuthServicesException
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public MetadataFailedValidationException()
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public MetadataFailedValidationException(string message)
            : base(message)
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public MetadataFailedValidationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Serialization Ctor
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Serialization context.</param>
        protected MetadataFailedValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
