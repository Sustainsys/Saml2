using Kentor.AuthServices.Internal;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Crypto description for a Managed implementation of SHA256 signatures.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SHA")]
    public class ManagedSHA256SignatureDescription : SignatureDescription
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ManagedSHA256SignatureDescription()
        {
            KeyAlgorithm = typeof(RSACryptoServiceProvider).FullName;
            DigestAlgorithm = typeof(SHA256Managed).FullName;
        }

        /// <summary>
        /// Create a deformatter
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Deformatter</returns>
        public override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var df = new RSAPKCS1SignatureDeformatter(key);
            df.SetHashAlgorithm(typeof(SHA256Managed).FullName);
            return df;
        }

        /// <summary>
        /// Create a formatter
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Formatter</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification ="No way from here find out when the created RSACRyptoServiceProvider is done. Have to leave it to GC/Finalizer.")]
        public override AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var provider = ((RSACryptoServiceProvider)key)
                .GetSha256EnabledRSACryptoServiceProvider();

            var formatter = new RSAPKCS1SignatureFormatter(provider);
            formatter.SetHashAlgorithm(typeof(SHA256Managed).FullName);
            return formatter;
        }
    }
}
