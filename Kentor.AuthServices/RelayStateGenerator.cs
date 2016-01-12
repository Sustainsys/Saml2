using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Generator of relay state for correlating requests.
    /// </summary>
    static class RelayStateGenerator
    {
        private static RNGCryptoServiceProvider random =
            new RNGCryptoServiceProvider();

        /// <summary>
        /// Create a unique random string that with a cryptographically secure
        /// random functions.
        /// </summary>
        /// <returns>Random string 56-chars string</returns>
        public static string CreateSecureKey()
        {
            var bytes = new byte[42];
            random.GetBytes(bytes);

            return Convert.ToBase64String(bytes)
                .Replace('/', '-')
                .Replace('+', '_');
        }
    }
}
