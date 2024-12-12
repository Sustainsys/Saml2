using Sustainsys.Saml2.Internal;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Sustainsys.Saml2
{

    /// <summary>
    /// Crypto description for a Managed implementation of SHA256 signatures.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SHA")]
	public abstract class ManagedRSASignatureDescription : SignatureDescription
	{
		public string HashAlgorithm { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        public ManagedRSASignatureDescription(int keyLength)
        {
            KeyAlgorithm = typeof(RSACryptoServiceProvider).FullName;
            switch (keyLength)
            {
	            case 256:
		            HashAlgorithm =  "sha256";
		            DigestAlgorithm = typeof(SHA256Managed).FullName;
		            break;
	            case 384:
		            HashAlgorithm =  "sha384";
		            DigestAlgorithm = typeof(SHA384Managed).FullName;
		            break;
	            case 512:
		            HashAlgorithm =  "sha512";
		            DigestAlgorithm = typeof(SHA512Managed).FullName;
		            break;
	            default:
		            throw new InvalidOperationException($"Unexpected SHA key length= {keyLength}");
            }
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
			df.SetHashAlgorithm(HashAlgorithm);
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

            var provider = ((RSA)key).GetSha256EnabledRSACryptoServiceProvider();

            var formatter = new RSAPKCS1SignatureFormatter(provider);
			formatter.SetHashAlgorithm(HashAlgorithm);
            return formatter;
        }
    }

	public class ManagedRSASHA256SignatureDescription : ManagedRSASignatureDescription
	{
		public ManagedRSASHA256SignatureDescription() : base(256)
		{
		}
	}

	public class ManagedRSASHA384SignatureDescription : ManagedRSASignatureDescription
	{
		public ManagedRSASHA384SignatureDescription() : base(384)
		{
		}
	}

	public class ManagedRSASHA512SignatureDescription : ManagedRSASignatureDescription
	{
		public ManagedRSASHA512SignatureDescription() : base(512)
		{
		}
	}
}
