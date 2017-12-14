using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Web;

namespace Kentor.AuthServices.StubIdp
{
    public class CertificateHelper
    {
        public static X509Certificate2 SigningCertificate
        {
            get
            {
                // If accessing on the legacy stubidp.kentor.se host name, use old certificate
                // to not break existing configured clients.
                if(HttpContext.Current.Request.Url.Host == "stubidp.kentor.se")
                {
                    return signingCertificateKentor;
                }
                return signingCertificate;
            }
        }

        // The X509KeyStorageFlags.MachineKeySet flag is required when loading a
        // certificate from file on a shared hosting solution such as Azure.
        private static readonly X509Certificate2 signingCertificateKentor
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
                // If accessing on the legacy stubidp.kentor.se host name, use old certificate
                // to not break existing configured clients.
                if (HttpContext.Current.Request.Url.Host == "stubidp.kentor.se")
                {
                    return signingKeyKentor;
                }
                return signingKey;

            }
        }

        private static readonly KeyDescriptor signingKeyKentor = 
            new KeyDescriptor(
            new SecurityKeyIdentifier(
                (new X509SecurityToken(signingCertificateKentor)).CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>()));

        private static readonly KeyDescriptor signingKey =
            new KeyDescriptor(
            new SecurityKeyIdentifier(
                (new X509SecurityToken(signingCertificate)).CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>()));

    }
}