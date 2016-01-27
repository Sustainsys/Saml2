using System;
using Kentor.AuthServices.Saml2P;
using System.Diagnostics.CodeAnalysis;

namespace Kentor.AuthServices.Internal
{
    internal static class NameIdFormatExtension
    {
        [ExcludeFromCodeCoverage]
        public static string GetString(this NameIdFormat nameIdFormat)
        {
            switch (nameIdFormat)
            {
                case NameIdFormat.Unspecified:
                    return "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified";
                case NameIdFormat.EmailAddress:
                    return "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress";
                case NameIdFormat.X509SubjectName:
                    return "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName";
                case NameIdFormat.WindowsDomainQualifiedName:
                    return "urn:oasis:names:tc:SAML:1.1:nameid-format:WindowsDomainQualifiedName";
                case NameIdFormat.KerberosPrincipalName:
                    return "urn:oasis:names:tc:SAML:2.0:nameid-format:kerberos";
                case NameIdFormat.EntityIdentifier:
                    return "urn:oasis:names:tc:SAML:2.0:nameid-format:entity";
                case NameIdFormat.Persistent:
                    return "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent";
                case NameIdFormat.Transient:
                    return "urn:oasis:names:tc:SAML:2.0:nameid-format:transient";
                default:
                    throw new ArgumentException("enum member does not exist", nameof(nameIdFormat));
            }
        }
    }
}