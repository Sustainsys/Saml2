using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2
{
    /// <summary>
    /// Generator of secure random keys..
    /// </summary>
    static class SecureKeyGenerator
    {
        private static RNGCryptoServiceProvider random =
            new RNGCryptoServiceProvider();

        /// <summary>
        /// Create a unique random string with a cryptographically secure
        /// random function.
        /// </summary>
        /// <returns>Random string 56-chars string</returns>
        public static string CreateRelayState()
        {
            // 16 is considered secure, but Base64 pads 16 bytes so
            // use 18 to make it even with Base64 that encodes multiples 
            // of 3 bytes)
            var bytes = new byte[18];
            random.GetBytes(bytes);

            return Convert.ToBase64String(bytes)
                .Replace('/', '-')
                .Replace('+', '_');
        }

        /// <summary>
        /// Create a unique random array with a cryptographically secure
        /// random function.
        /// </summary>
        /// <returns>20 random bytes.</returns>
        public static byte[] CreateArtifactMessageHandle()
        {
            var bytes = new byte[20];
            random.GetBytes(bytes);

            return bytes;
        }
    }
}
