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
        HttpGet = 1,

        /// <summary>
        /// The data was retrevied through TLS protected request that was 
        /// initaited from our end, to a host that had a valid TLS certificate.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Tls")]
        TlsTransport = 2,

        /// <summary>
        /// The data was signed and have been verified by a signing key. The 
        /// data integrity algorithm is 160 bit Sha-1.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sha")]
        SignatureSha160 = 3,

        // Deliberately made room for more values as more hash algorithms
        // are introduced.

        /// <summary>
        /// Data is from a local configuration source. E.g. metadata or a
        /// certificate loaded from disk.
        /// </summary>
        LocalConfiguration = 100
    }
}
