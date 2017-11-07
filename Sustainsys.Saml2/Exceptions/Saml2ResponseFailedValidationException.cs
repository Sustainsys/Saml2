using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Exceptions
{
    /// <summary>
    /// A SAML2 Response failed validation.
    /// </summary>
    [Serializable]
    public class Saml2ResponseFailedValidationException : AuthServicesException
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public Saml2ResponseFailedValidationException()
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public Saml2ResponseFailedValidationException(string message) : base(message)
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public Saml2ResponseFailedValidationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Serialization Ctor
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected Saml2ResponseFailedValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
