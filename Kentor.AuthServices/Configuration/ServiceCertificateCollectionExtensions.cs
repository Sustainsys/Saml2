using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Extension methods ICollection&lt;ServiceCertificate&gt;
    /// </summary>
    public static class ServiceCertificateCollectionExtensions
    {
        /// <summary>
        /// Add a certificate to the collection with default status use and
        /// metadata behaviour.
        /// </summary>
        /// <param name="collection">collection to add to.</param>
        /// <param name="certificate">Certificate to add.</param>
        public static void Add(
            this ICollection<ServiceCertificate> collection,
            X509Certificate2 certificate)
        {
            if(collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            collection.Add(new ServiceCertificate()
            {
                Certificate = certificate
            });
        }
    }
}
