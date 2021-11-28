using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using EncryptingCredentials = Microsoft.IdentityModel.Tokens.EncryptingCredentials;
using SecurityAlgorithms = Microsoft.IdentityModel.Tokens.SecurityAlgorithms;

namespace Sustainsys.Saml2.Metadata.Extensions
{
    // TODO : Refactor as there is duplicate in the Sustainsys.Saml2 assembly
    // for not set to internal to prevent conflict
    internal static class AsymmetricAlgorithmExtensions
    {
        private const string RSA = "1.2.840.113549.1.1.1";
        private const string DSA = "1.2.840.10040.4.1";
        private const string ECC = "1.2.840.10045.2.1";

        internal static AsymmetricAlgorithm GetPrivateKey(this X509Certificate2 cert)
        {
            switch (cert.PublicKey.Oid.Value)
            {
                case RSA:
                    return cert.GetRSAPrivateKey();
                case ECC:
                    return cert.GetECDsaPrivateKey();
#if NET472_OR_GREATER
                case DSA:
                    return cert.GetDSAPrivateKey();
#endif
                default:
                    return cert.PrivateKey;
            }
        }
    }
}