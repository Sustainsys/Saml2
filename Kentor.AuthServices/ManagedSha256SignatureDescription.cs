using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

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

            var provider = (RSACryptoServiceProvider)key;

            // The provider is probably using the default ProviderType. That's
            // a problem, because it doesn't support SHA256. Let's do some
            // black magic and create a new provider of a type that supports
            // SHA256 without the user ever knowing we fix this. This is what 
            // is done in X509AsymmetricKey.GetSignatureFormatter if 
            // http://www.w3.org/2001/04/xmldsig-more#rsa-sha256 isn't
            // a known algorithm, so users kind of expect this to be handled
            // for them magically.

            var cspParams = new CspParameters();
            cspParams.ProviderType = 24; //PROV_RSA_AES
            cspParams.KeyContainerName = provider.CspKeyContainerInfo.KeyContainerName;
            cspParams.KeyNumber = (int)provider.CspKeyContainerInfo.KeyNumber;
            SetMachineKeyFlag(provider, cspParams);

            cspParams.Flags |= CspProviderFlags.UseExistingKey;

            provider = new RSACryptoServiceProvider(cspParams);

            var f = new RSAPKCS1SignatureFormatter(provider);
            f.SetHashAlgorithm(typeof(SHA256Managed).FullName);
            return f;
        }

        // We don't want to use Machine Key store during tests, so let's
        // put this one in an own method that's not included in coverage metrics.
        [ExcludeFromCodeCoverage]
        private static void SetMachineKeyFlag(RSACryptoServiceProvider provider, CspParameters cspParams)
        {
            if (provider.CspKeyContainerInfo.MachineKeyStore)
            {
                cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            }
        }
    }
}
