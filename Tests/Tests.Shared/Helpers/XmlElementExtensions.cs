using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Metadata.Tokens;
using Sustainsys.Saml2.Tokens;

namespace Sustainsys.Saml2.Tests.Helpers
{
    static class XmlElementExtensions
    {
        /// <summary>
        /// Checks if an xml element is signed by the given certificate, through
        /// a contained enveloped signature. Helper for tests. Production
        /// code always should handle multiple possible signing keys.
        /// </summary>
        /// <param name="xmlElement">Xml Element that should be signed</param>
        /// <param name="certificate">Certificate that should validate</param>
        /// <returns>Is the signature correct?</returns>
        internal static bool IsSignedBy(
            this XmlElement xmlElement,
            X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            return xmlElement.IsSignedByAny(
                Enumerable.Repeat(new X509RawDataKeyIdentifierClause(certificate), 1),
                false,
                SignedXml.XmlDsigRSASHA1Url);
        }
    }
}
