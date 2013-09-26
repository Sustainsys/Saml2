using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config element for the signing certificate.
    /// </summary>
    public class CertificateElement : ConfigurationElement
    {
        [ConfigurationProperty("fileName")]
        public string FileName
        {
            get
            {
                return (string)this["fileName"];
            }
        }

        /// <summary>
        /// Store name to search.
        /// </summary>
        [ConfigurationProperty("storeName")]
        public StoreName StoreName
        {
            get
            {
                return (StoreName)this["storeName"];
            }
        }

        /// <summary>
        /// Store location to search.
        /// </summary>
        [ConfigurationProperty("storeLocation")]
        public StoreLocation StoreLocation
        {
            get
            {
                return (StoreLocation)this["storeLocation"];
            }
        }

        /// <summary>
        /// The search term used for searching the certificate store.
        /// </summary>
        [ConfigurationProperty("findValue")]
        public string FindValue
        {
            get
            {
                return (string)this["findValue"];
            }
        }

        /// <summary>
        /// Find type, what field to search.
        /// </summary>
        [ConfigurationProperty("x509FindType")]
        public X509FindType X509FindType
        {
            get
            {
                return (X509FindType)this["x509FindType"];
            }
        }

        public X509Certificate2 LoadCertificate()
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                return new X509Certificate2(FileName);
            }
            else
            {
                var store = new X509Store(StoreName, StoreLocation);              
                store.Open(OpenFlags.ReadOnly);
                try
                {
                    var certs = store.Certificates.Find(X509FindType, FindValue, false);

                    if (certs.Count != 1)
                    {
                        throw new InvalidOperationException(
                            string.Format(CultureInfo.InvariantCulture, 
                            "Finding cert through {0} in {1}:{2} with value {3} matched {4} certificates. A unique match is required.",
                            X509FindType, StoreLocation, StoreName, FindValue, certs.Count));
                    }

                    return certs[0];
                }
                finally
                {
                    store.Close();
                }
            }
        }
    }
}
