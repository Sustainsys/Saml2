using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Exceptions
{
    /// <summary>
    /// Exception thrown when an signature is not valid according to the
    /// SAML standard.
    /// </summary>
    [Serializable]
    public class InvalidSignatureException : AuthServicesException
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
            : base( message, innerException)
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
