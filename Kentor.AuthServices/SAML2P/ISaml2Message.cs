using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// Common properties of all Saml2 message implementations (both requests
    /// and responses). There is no corresponding definition in the SAML2
    /// standard, so this is made up of the common fields of 3.2.2 Complex Type
    /// StatusResponseType (the base type for all responses) and of 3.2.1 Complex
    /// Type RequestAbstractType.
    /// </summary>
    public interface ISaml2Message
    {
        /// <summary>
        /// The destination of the message.
        /// </summary>
        Uri DestinationUrl { get; }

        /// <summary>
        /// Serializes the message into wellformed Xml.
        /// </summary>
        /// <returns>string containing the Xml data.</returns>
        string ToXml();

        /// <summary>
        /// The name of the message to use in a query string or form input
        /// field. Typically "SAMLRequest" or "SAMLResponse".
        /// </summary>
        string MessageName { get; }
    }
}
