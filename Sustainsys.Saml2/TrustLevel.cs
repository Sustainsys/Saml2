using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    /// <summary>
    /// The level of trust that a certain piece of data comes with.
    /// </summary>
    public enum TrustLevel
    {
        /// <summary>
        /// The data cannot be trusted at all.
        /// </summary>
        None = 0,

        /// <summary>
        /// The data was retreived through a request that was initiated from
        /// our end, but there was no transport protection.
        /// </summary>
        HttpGet = 100,

        /// <summary>
        /// The data was retrevied through TLS protected request that was 
        /// initaited from our end, to a host that had a valid TLS certificate.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Tls")]
        TlsTransport = 200,

        /// <summary>
        /// The data was signed and have been verified by a signing key.
        /// </summary>
        Signature = 300,

        /// <summary>
        /// Data is from a local configuration source. E.g. metadata or a
        /// certificate loaded from disk.
        /// </summary>
        LocalConfiguration = 1000
    }
}
