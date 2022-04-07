using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Metadata.Xml;

/// <summary>
/// What is the trust level of a piece of data? The levels reflect
/// how trustworthy the data is based on if it is signed and how
/// the signature can be validated.
/// </summary>
public enum TrustLevel
{
    /// <summary>
    /// There is no integrity protection for the data.
    /// </summary>
    None,

    /// <summary>
    /// The data was retreived over an outbound network connection,
    /// but the transport was not protected. This level is also set
    /// on all data that is verified as signed by a key that was retrieved
    /// over plain http.
    /// </summary>
    Http,

    /// <summary>
    /// The data was directly retreived from the source using a valid
    /// TLS (https) connection. This level is also set on all data that
    /// is verified as signed by a key that was retrieved over TLS/https.
    /// In most setups, this level is regarded as secure.
    /// </summary>
    TLS,

    /// <summary>
    /// The data was verified by a signature where signing key or a strong
    /// identifier of the key (such as a SAH256 cert thumbprint) was read 
    /// from configuration.
    /// </summary>
    ConfiguredKey
}
