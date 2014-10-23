using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Kentor.AuthServices
{
    /// <summary>
    /// SAML2 namespace constants.
    /// </summary>
    public static class Saml2Namespaces
    {
        /// <summary>
        /// Namespace of the SAML2 protocol.
        /// </summary>
        public const string Saml2PName = "urn:oasis:names:tc:SAML:2.0:protocol";

        /// <summary>
        /// Namespace of the SAML2 protocol.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly XNamespace Saml2P = XNamespace.Get(Saml2PName);

        /// <summary>
        /// Namespace of SAML2 assertions.
        /// </summary>
        public const string Saml2Name = "urn:oasis:names:tc:SAML:2.0:assertion";

        /// <summary>
        /// Namespace of SAML2 assertions.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly XNamespace Saml2 = XNamespace.Get(Saml2Name);

        /// <summary>
        /// Namespace of SAML2 Metadata.
        /// </summary>
        public const string Saml2MetadataName = "urn:oasis:names:tc:SAML:2.0:metadata";

        /// <summary>
        /// Namespace of SAML2 Metadata.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly XNamespace Saml2Metadata = XNamespace.Get(Saml2MetadataName);

        /// <summary>
        /// Namespace for idp discovery protocol extension.
        /// </summary>
        public const string Saml2IdpDiscoveryName = "urn:oasis:names:tc:SAML:profiles:SSO:idp-discovery-protocol";

        /// <summary>
        /// Namespace for idp discovery protocol extension.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly XNamespace Saml2IdpDiscovery = XNamespace.Get(Saml2IdpDiscoveryName);

        /// <summary>
        /// Namespace for Xml schema instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly XNamespace XmlSchemaInstance = XNamespace.Get(System.Xml.Schema.XmlSchema.InstanceNamespace);
    }
}
