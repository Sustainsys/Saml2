using System;
using System.Runtime.Serialization;

namespace Kentor.AuthServices.Exceptions
{
    /// <summary>
    /// Base class for authentication services specific exceptions, that might                     
    /// require special handling for error reporting to the user.
    /// </summary>
    [Serializable]
    public abstract class AuthServicesException : Exception
    {
        /// <summary>
        /// Default Ctor
        /// </summary>
        protected AuthServicesException() { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        protected AuthServicesException(string message)
            : base(message)
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception.</param>
        protected AuthServicesException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Serialization Ctor
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected AuthServicesException(SerializationInfo info, StreamingContext context)
            :base(info, context)
        { }
    }
}
