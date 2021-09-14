using Microsoft.IdentityModel.Tokens;
using Sustainsys.Saml2.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using CipherData = System.Security.Cryptography.Xml.CipherData;
using EncryptedData = System.Security.Cryptography.Xml.EncryptedData;
using EncryptingCredentials = Microsoft.IdentityModel.Tokens.EncryptingCredentials;
using EncryptionMethod = System.Security.Cryptography.Xml.EncryptionMethod;
using SecurityAlgorithms = Microsoft.IdentityModel.Tokens.SecurityAlgorithms;

namespace Sustainsys.Saml2.Internal
{
    internal static class CryptographyExtensions
    {
        private const String RSA = "1.2.840.113549.1.1.1";
        private const String DSA = "1.2.840.10040.4.1";
        private const String ECC = "1.2.840.10045.2.1";

        internal static void Encrypt(this XmlElement elementToEncrypt, EncryptingCredentials encryptingCredentials)
        {
            if (elementToEncrypt == null)
            {
                throw new ArgumentNullException(nameof(elementToEncrypt));
            }
            if (encryptingCredentials == null)
            {
                throw new ArgumentNullException(nameof(encryptingCredentials));
            }

            string enc;
            int keySize;
            switch (encryptingCredentials.Enc)
            {
                case SecurityAlgorithms.Aes128CbcHmacSha256:
                    enc = EncryptedXml.XmlEncAES128Url;
                    keySize = 128;
                    break;

                case SecurityAlgorithms.Aes192CbcHmacSha384:
                    enc = EncryptedXml.XmlEncAES192Url;
                    keySize = 192;
                    break;

                case SecurityAlgorithms.Aes256CbcHmacSha512:
                    enc = EncryptedXml.XmlEncAES256Url;
                    keySize = 256;
                    break;

                default:
                    throw new CryptographicException(
                        $"Unsupported cryptographic algorithm {encryptingCredentials.Enc}");
            }

            var encryptedData = new EncryptedData
            {
                Type = EncryptedXml.XmlEncElementUrl,
                EncryptionMethod = new EncryptionMethod(enc)
            };

            string alg;
            switch (encryptingCredentials.Alg)
            {
                case SecurityAlgorithms.RsaOAEP:
                    alg = EncryptedXml.XmlEncRSAOAEPUrl;
                    break;

                case SecurityAlgorithms.RsaPKCS1:
                    alg = EncryptedXml.XmlEncRSA15Url;
                    break;

                default:
                    throw new CryptographicException(
                        $"Unsupported cryptographic algorithm {encryptingCredentials.Alg}");
            }
            var encryptedKey = new EncryptedKey
            {
                EncryptionMethod = new EncryptionMethod(alg),
            };

            var encryptedXml = new EncryptedXml();
            byte[] encryptedElement;
            using (var symmetricAlgorithm = new RijndaelManaged())
            {
                X509SecurityKey x509SecurityKey = encryptingCredentials.Key as X509SecurityKey;
                if (x509SecurityKey == null)
                {
                    throw new CryptographicException(
                        "The encrypting credentials have an unknown key of type {encryptingCredentials.Key.GetType()}");
                }

                symmetricAlgorithm.KeySize = keySize;
                encryptedKey.CipherData = new CipherData(EncryptedXml.EncryptKey(symmetricAlgorithm.Key,
                    (RSA)x509SecurityKey.PublicKey, alg == EncryptedXml.XmlEncRSAOAEPUrl));
                encryptedElement = encryptedXml.EncryptData(elementToEncrypt, symmetricAlgorithm, false);
            }
            encryptedData.CipherData.CipherValue = encryptedElement;

            encryptedData.KeyInfo = new KeyInfo();
            encryptedData.KeyInfo.AddClause(new KeyInfoEncryptedKey(encryptedKey));
            EncryptedXml.ReplaceElement(elementToEncrypt, encryptedData, false);
        }

        internal static void Encrypt(this XmlElement elementToEncrypt, bool useOaep, X509Certificate2 certificate)
        {
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));

            var encryptedData = new EncryptedData
            {
                Type = EncryptedXml.XmlEncElementUrl,
                EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url)
            };

            var algorithm = useOaep ? EncryptedXml.XmlEncRSAOAEPUrl : EncryptedXml.XmlEncRSA15Url;
            var encryptedKey = new EncryptedKey
            {
                EncryptionMethod = new EncryptionMethod(algorithm),
            };

            var encryptedXml = new EncryptedXml();
            byte[] encryptedElement;
            using (var symmetricAlgorithm = new RijndaelManaged())
            {
                symmetricAlgorithm.KeySize = 256;
                encryptedKey.CipherData = new CipherData(EncryptedXml.EncryptKey(symmetricAlgorithm.Key, (RSA)certificate.PublicKey.Key, useOaep));
                encryptedElement = encryptedXml.EncryptData(elementToEncrypt, symmetricAlgorithm, false);
            }
            encryptedData.CipherData.CipherValue = encryptedElement;

            encryptedData.KeyInfo = new KeyInfo();
            encryptedData.KeyInfo.AddClause(new KeyInfoEncryptedKey(encryptedKey));
            EncryptedXml.ReplaceElement(elementToEncrypt, encryptedData, false);
        }

        internal static IEnumerable<XmlElement> Decrypt(this IEnumerable<XmlElement> elements, AsymmetricAlgorithm key)
        {
            foreach (var element in elements)
            {
                yield return element.Decrypt(key);
            }
        }

        internal static XmlElement Decrypt(this XmlElement element, AsymmetricAlgorithm key)
        {
            var xmlDoc = XmlHelpers.XmlDocumentFromString(element.OuterXml);

            var exml = new RSAEncryptedXml(xmlDoc, (RSA)key);

            exml.DecryptDocument();

            return xmlDoc.DocumentElement;
        }

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

        // We don't want to use Machine Key store during tests, so let's
        // put this one in an own method that's not included in coverage metrics.
        [ExcludeFromCodeCoverage]
        private static void SetMachineKeyFlag(RSACryptoServiceProvider provider, CspParameters cspParams)
        {
            if (provider.CspKeyContainerInfo.MachineKeyStore)
            {
                cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            }
        }

        // CryptoConfig.CreateFromName doesn't know about these
        private static Dictionary<string, Type> s_extraAlgorithms = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            {  SecurityAlgorithms.RsaSha256Signature, typeof(ManagedRSASHA256SignatureDescription) },
            {  SecurityAlgorithms.RsaSha384Signature, typeof(ManagedRSASHA384SignatureDescription) },
            {  SecurityAlgorithms.RsaSha512Signature, typeof(ManagedRSASHA512SignatureDescription) }
        };

        public static object CreateAlgorithmFromName(string name, params object[] args)
        {
            var result = CryptoConfig.CreateFromName(name);
            if (result != null)
            {
                return result;
            }

            Type type;
            if (!s_extraAlgorithms.TryGetValue(name, out type))
            {
                throw new CryptographicException($"Unknown crypto algorithm '{name}'");
            }
            return Activator.CreateInstance(type, args);
        }
    }
}