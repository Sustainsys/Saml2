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
    /// Allowed hash algorithms if validating signatures.
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
    /// Read a NameID
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <returns>NameId</returns>
    NameId ReadNameId(XmlTraverser source);

    /// <summary>
    /// Read a Saml response
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <returns>SamlResponse</returns>
    SamlResponse ReadSamlResponse(XmlTraverser source);

    /// <summary>
    /// Read an AuthnReqeust
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <returns>AutnnRequest</returns>
    AuthnRequest ReadAuthnRequest(XmlTraverser source);
}
