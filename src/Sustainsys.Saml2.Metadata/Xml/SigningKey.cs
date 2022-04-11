using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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
    public X509Certificate2? Certificate { get; set; }

    /// <summary>
    /// TrustLevel of the key.
    /// </summary>
    public TrustLevel TrustLevel { get; init;}
}
