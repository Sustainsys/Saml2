using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Kentor.AuthServices.Internal
{
    internal class RSAEncryptedXml : EncryptedXml
    {
        private readonly RSA privateKey;

        public RSAEncryptedXml(XmlDocument document, RSA rsaKey)
            : base(document)
        {
            privateKey = rsaKey;
        }

        // Try to decrypt the EncryptedKey by the given key
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CipherReference"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CipherData")]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
        public override byte[] DecryptEncryptedKey(EncryptedKey encryptedKey)
        {
            if (encryptedKey == null)
            {
                throw new ArgumentNullException(nameof(encryptedKey));
            }

            if (encryptedKey.CipherData.CipherValue == null)
            {
                throw new NotImplementedException("Unable to decode CipherData of type \"CipherReference\".");
            }

            // use the key info
            if (privateKey == null)
            {
                return base.DecryptEncryptedKey(encryptedKey);
            }

            var useOaep = (encryptedKey.EncryptionMethod != null &&
                         encryptedKey.EncryptionMethod.KeyAlgorithm == XmlEncRSAOAEPUrl);

            return DecryptKey(encryptedKey.CipherData.CipherValue, privateKey, useOaep);
        }
    }
}
