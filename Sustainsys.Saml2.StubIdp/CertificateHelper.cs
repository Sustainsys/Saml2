using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Web;

namespace Sustainsys.Saml2.StubIdp
{
    public class CertificateHelper
    {
        public static X509Certificate2 SigningCertificate
        {
            get
            {
                // If accessing on the legacy stubidp.Sustainsys.se host name, use old certificate
                // to not break existing configured clients.
                if(HttpContext.Current.Request.Url.Host == "stubidp.Sustainsys.se")
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
                "~\\App_Data\\Sustainsys.Saml2.StubIdp.pfx"), "",
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
                // If accessing on the legacy stubidp.Sustainsys.se host name, use old certificate
                // to not break existing configured clients.
                if (HttpContext.Current.Request.Url.Host == "stubidp.Sustainsys.se")
                {
                    return signingKeySustainsys;
                }
                return signingKey;

            }
        }

        private static readonly KeyDescriptor signingKeySustainsys = 
            new KeyDescriptor(
            new SecurityKeyIdentifier(
                (new X509SecurityToken(signingCertificateSustainsys)).CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>()));

        private static readonly KeyDescriptor signingKey =
            new KeyDescriptor(
            new SecurityKeyIdentifier(
                (new X509SecurityToken(signingCertificate)).CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>()));

    }
}