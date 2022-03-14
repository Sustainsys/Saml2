using System;
using System.Collections.Generic;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Web;
using Sustainsys.Saml2.Metadata.Descriptors;

namespace Sustainsys.Saml2.StubIdp
{
    public class CertificateHelper
    {
        public static X509Certificate2 SigningCertificate
        {
            get
            {
                // If accessing on the legacy stubidp.Kentor.se host name, use old certificate
                // to not break existing configured clients.
                if(HttpContext.Current.Request.Url.Host == "stubidp.kentor.se")
                {
                    return signingCertificateSustainsys;
                }
                return signingCertificate;
            }
        }

        // The X509KeyStorageFlags.MachineKeySet flag is required when loading a
        // certificate from file on a shared hosting solution such as Azure.
        private static readonly X509Certificate2 signingCertificateSustainsys
            = new X509Certificate2(
                HttpContext.Current.Server.MapPath(
                "~\\App_Data\\Kentor.AuthServices.StubIdp.pfx"), "",
                X509KeyStorageFlags.MachineKeySet);

        private static readonly X509Certificate2 signingCertificate
            = new X509Certificate2(
                HttpContext.Current.Server.MapPath(
                "~\\App_Data\\stubidp.sustainsys.com.pfx"), "",
                X509KeyStorageFlags.MachineKeySet);

        public static KeyDescriptor SigningKey
        {
            get
            {
                // If accessing on the legacy stubidp.Kentor.se host name, use old certificate
                // to not break existing configured clients.
                if (HttpContext.Current.Request.Url.Host == "stubidp.kentor.se")
                {
                    return signingKeySustainsys;
                }
                return signingKey;

            }
        }

		static KeyDescriptor CreateKeyDescriptor(X509Certificate2 cert)
		{
			var keyDescriptor = new KeyDescriptor();
			keyDescriptor.KeyInfo = new DSigKeyInfo();
			var x509Data = new X509Data();
			x509Data.Certificates.Add(cert);
			keyDescriptor.KeyInfo.Data.Add(x509Data);
			return keyDescriptor;
		}

		private static readonly KeyDescriptor signingKeySustainsys =
			CreateKeyDescriptor(signingCertificateSustainsys);

		private static readonly KeyDescriptor signingKey =
			CreateKeyDescriptor(signingCertificate);

    }
}