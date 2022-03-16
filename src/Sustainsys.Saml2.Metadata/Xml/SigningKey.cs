using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Metadata.Xml;

/// <summary>
/// Represents a signing key.
/// </summary>
public class SigningKey
{
    /// <summary>
    /// The asymmetric algorithm.
    /// </summary>
    public AsymmetricAlgorithm? Key { get; set; }

    /// <summary>
    /// TrustLevel of the key.
    /// </summary>
    public TrustLevel TrustLevel { get; init;}
}
