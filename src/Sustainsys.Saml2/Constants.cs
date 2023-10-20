using Sustainsys.Saml2.Samlp;
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

    /// <summary>
    /// Standard status code URIs
    /// </summary>
    public static class StatusCodes
    {
        /// <summary>
        /// Failed because of the requester, i.e. invalid request.
        /// </summary>
        public const string Requester = "urn:oasis:names:tc:SAML:2.0:status:Requester";

        /// <summary>
        /// Success
        /// </summary>
        public const string Success = "urn:oasis:names:tc:SAML:2.0:status:Success";
    }

    /// <summary>
    /// Names of elements
    /// </summary>
    public static class Elements
    {
        /// <summary>
        /// Issuer XML element name.
        /// </summary>
        public const string Issuer = nameof(Issuer);

        /// <summary>
        /// Response XML element name.
        /// </summary>
        public const string Response = nameof(Response);

        /// <summary>
        /// Status XML element name.
        /// </summary>
        public const string Status = nameof(Status);

        /// <summary>
        /// StatusCode XML element name.
        /// </summary>
        public const string StatusCode = nameof(StatusCode);

        /// <summary>
        /// Extensions XML element name.
        /// </summary>
        public const string Extensions = nameof(Extensions);

        /// <summary>
        /// EntityDescriptor XML element name.
        /// </summary>
        public const string EntityDescriptor = nameof(EntityDescriptor);

        /// <summary>
        /// RoleDescriptor XML element name.
        /// </summary>
        public const string RoleDescriptor = nameof(RoleDescriptor);

        /// <summary>
        /// IDPSSODescriptor XML element name.
        /// </summary>
        public const string IDPSSODescriptor = nameof(IDPSSODescriptor);

        /// <summary>
        /// SingleSignOnService XML element name.
        /// </summary>
        public const string SingleSignOnService = nameof(SingleSignOnService);

        /// <summary>
        /// Signature XML element name
        /// </summary>
        public const string Signature = nameof(Signature);

        /// <summary>
        /// Keydescriptro XML element name.
        /// </summary>
        public const string KeyDescriptor = nameof(KeyDescriptor);

        /// <summary>
        /// Organization XML element name.
        /// </summary>
        public const string Organization = nameof(Organization);

        /// <summary>
        /// ContactPerson XML element name.
        /// </summary>
        public const string ContactPerson = nameof(ContactPerson);

        /// <summary>
        /// ArtifactResolutionService XML element name.
        /// </summary>
        public const string ArtifactResolutionService = nameof(ArtifactResolutionService);

        /// <summary>
        /// SingleLogoutService XML element name.
        /// </summary>
        public const string SingleLogoutService = nameof(SingleLogoutService);

        /// <summary>
        /// ManageNameIDService XML element name.
        /// </summary>
        public const string ManageNameIDService = nameof(ManageNameIDService);

        /// <summary>
        /// NameIDFormat XML element name.
        /// </summary>
        public const string NameIDFormat = nameof(NameIDFormat);

        /// <summary>
        /// KeyInfo XML element name.
        /// </summary>
        public const string KeyInfo = nameof(KeyInfo);

        /// <summary>
        /// SPSSODescriptor XML element name.
        /// </summary>
        public const string SPSSODescriptor = nameof(SPSSODescriptor);

        /// <summary>
        /// AuthnAuthorityDescriptor XML element name.
        /// </summary>
        public const string AuthnAuthorityDescriptor = nameof(AuthnAuthorityDescriptor);

        /// <summary>
        /// AttributeAuthorityDescriptor XML element name.
        /// </summary>
        public const string AttributeAuthorityDescriptor = nameof(AttributeAuthorityDescriptor);

        /// <summary>
        /// PDPDescriptor XML element name.
        /// </summary>
        public const string PDPDescriptor = nameof(PDPDescriptor);
    }
}
