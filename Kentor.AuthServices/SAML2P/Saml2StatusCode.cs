using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// Status codes, mapped against states in section 3.2.2.2 in the SAML2 spec.
    /// </summary>
    public enum Saml2StatusCode
    {
        /// <summary>
        /// Success.
        /// </summary>
        Success,

        /// <summary>
        /// Error because of the requester.
        /// </summary>
        Requester,

        /// <summary>
        /// Error because of the responder.
        /// </summary>
        Responder,

        /// <summary>
        /// Versions doesn't match.
        /// </summary>
        VersionMismatch,
    }
}
