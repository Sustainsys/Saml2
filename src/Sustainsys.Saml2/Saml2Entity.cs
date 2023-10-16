using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
