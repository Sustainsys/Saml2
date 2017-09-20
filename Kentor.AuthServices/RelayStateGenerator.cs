using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
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
            var bytes = new byte[42];
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
