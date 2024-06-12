using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2;
/// <summary>
/// A Saml2 entity, i.e. an Identity Provider or a Service Provider
/// </summary>
public class Saml2Entity
{
    /// <summary>
    /// The entity id of the identity provider
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// Allowed hash algorithms if validating signatures.
    /// </summary>
    public IEnumerable<string>? AllowedHashAlgorithms { get; set; }

    /// <summary>
    /// Signing keys to trust when validating signatures of the metadata.
    /// </summary>
    public IEnumerable<SigningKey>? TrustedSigningKeys { get; set; }

}
