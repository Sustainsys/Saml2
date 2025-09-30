using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.Serialization;

/// <summary>
/// Reader for Saml classes from Xml
/// </summary>
public interface ISamlXmlReader
{
    /// <summary>
    /// Allowed hash algorithms if validating signatures. Values should be e.g. "sha256"
    /// which is compared to the end of the algorithm identifier Url.
    /// </summary>
    IEnumerable<string>? AllowedHashAlgorithms { get; set; }

    /// <summary>
    /// Signing keys to trust when validating signatures of the metadata. In addition
    /// to these, the signing keys configured for a known issuer are considered. This
    /// property is mostly useful for validation of signed metadata.
    /// </summary>
    IEnumerable<SigningKey>? TrustedSigningKeys { get; set; }

    /// <summary>
    /// Called when information about a Saml entity is needed, e.g. to get the signing
    /// keys configured for an entity.
    /// </summary>
    Func<string, Saml2Entity>? EntityResolver { get; set; }

    /// <summary>
    /// Read an Entity Descriptor
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <param name="errorInspector">Callback that can inspect and alter errors before throwing</param>
    /// <returns>EntityDescriptor</returns>
    EntityDescriptor ReadEntityDescriptor(
        XmlTraverser source,
        Action<ReadErrorInspectorContext<EntityDescriptor>>? errorInspector = null);

    /// <summary>
    /// Read a Saml response
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <param name="errorInspector">Callback that can inspect and alter errors before throwing</param>
    /// <returns>SamlResponse</returns>
    Response ReadResponse(
        XmlTraverser source,
        Action<ReadErrorInspectorContext<Response>>? errorInspector = null);

    /// <summary>
    /// Read an <see cref="AuthnRequest"/>
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <param name="errorInspector">Callback that can inspect and alter errors before throwing</param>
    /// <returns><see cref="AuthnRequest"/></returns>
    AuthnRequest ReadAuthnRequest(
        XmlTraverser source,
        Action<ReadErrorInspectorContext<AuthnRequest>>? errorInspector = null);

    /// <summary>
    /// Read an <see cref="Assertion"/>
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <param name="errorInspector">Callback that can inspect and alter errors before throwing</param>
    /// <returns><see cref="Assertion"/></returns>
    Assertion ReadAssertion(
        XmlTraverser source,
        Action<ReadErrorInspectorContext<Assertion>>? errorInspector = null);
}