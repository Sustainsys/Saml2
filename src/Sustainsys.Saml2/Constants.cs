using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2;

/// <summary>
/// Constants
/// </summary>
public static class Constants
{
    /// <summary>
    /// Namespace URIs
    /// </summary>
    public static class Namespaces
    {
        /// <summary>
        /// Namespace uri for the saml 2.0 protocol namespace.
        /// </summary>
        public const string Samlp = "urn:oasis:names:tc:SAML:2.0:protocol";

        /// <summary>
        /// Namespace uri for the saml 2.0 assertion namespace.
        /// </summary>
        public const string Saml = "urn:oasis:names:tc:SAML:2.0:assertion";

        /// <summary>
        /// Namespace uri for the saml 2.0 Metadata namespace.
        /// </summary>
        public const string Metadata = "urn:oasis:names:tc:SAML:2.0:metadata";
    }

    /// <summary>
    /// SAMLRequest
    /// </summary>
    public const string SamlRequest = "SAMLRequest";

    /// <summary>
    /// SAMLResponse
    /// </summary>
    public const string SamlResponse = "SAMLResponse";

    /// <summary>
    /// RelayState
    /// </summary>
    public const string RelayState = "RelayState";

    /// <summary>
    /// String constants for binding Uris.
    /// </summary>
    public static class BindingUris
    {
        /// <summary>
        /// HTTP Redirect binding identifier.
        /// </summary>
        public const string HttpRedirect = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect";

        /// <summary>
        /// HTTP POST binding identifier.
        /// </summary>
        public const string HttpPOST = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST";

        /// <summary>
        /// SOAP binding identifier.
        /// </summary>
        public const string SOAP = "urn:oasis:names:tc:SAML:2.0:bindings:SOAP";
    }
}
