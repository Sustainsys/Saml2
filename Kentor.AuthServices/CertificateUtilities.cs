using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace Kentor.AuthServices
{
    class CertificateUtilities
    {
        /// <summary>
        /// Search for the specified certificate. If more than one certificate is found then an <see cref="InvalidOperationException"/> is thrown.
        /// </summary>
        /// <param name="name">Name of the store to search.</param>
        /// <param name="location">The location in the certificate store to search in.</param>
        /// <param name="findType">The type of search or query to match on.</param>
        /// <param name="value">The value to find.</param>
        /// <param name="certificate">The certificate if found, null if not found or an exception is thrown.</param>
        /// <returns>Treu if the certificate was found, false if otherwise.</returns>
        public static bool TryGetCertificate(StoreName name, StoreLocation location, X509FindType findType, string value, out X509Certificate2 certificate)
        {
            certificate = null;

            var store = new X509Store(name, location);
            store.Open(OpenFlags.ReadOnly);
            try {
                var certs = store.Certificates.Find(findType, value, false);
                if (certs.Count == 1) {
                    certificate = certs[0];
                    return (true);
                }
                if (certs.Count > 1) {
                    throw new CertificateNotFoundException(
                        string.Format(CultureInfo.InvariantCulture,
                        "Finding cert through {0} in {1}:{2} with value {3} matched {4} certificates. A unique match is required.",
                        findType, location, name, value, certs.Count));
                }
            } finally {
                store.Close();
            }
            return false;
        }

        /// <summary>
        /// Search for the specified certificate. If certificate is not found, then a <see cref="CertificateNotFoundException" /> is thrown. 
        /// If more than one certificate is found then an <see cref="InvalidOperationException"/> is thrown.
        /// </summary>
        /// <param name="name">Name of the store to search.</param>
        /// <param name="location">The location in the certificate store to search in.</param>
        /// <param name="findType">The type of search or query to match on.</param>
        /// <param name="value">The value to find.</param>
        /// <returns>The certificate if found. If not found, then a <see cref="CertificateNotFoundException" /> is thrown.</returns>
        public static X509Certificate2 GetCertificate(StoreName name, StoreLocation location, X509FindType findType, string value)
        {
            X509Certificate2 certificate;
            if (TryGetCertificate(name, location, findType, value, out certificate)) {
                return certificate;
            }
            
            throw new CertificateNotFoundException(
                        string.Format(CultureInfo.InvariantCulture,
                        "Finding cert through {0} in {1}:{2} with value {3} matched no certificates.",
                        findType, location, name, value));
        }
    }
}
