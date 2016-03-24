using System;
using System.IdentityModel.Tokens;
using System.Xml.Linq;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Extension methods for Saml2NameId
    /// </summary>
    public static class Saml2NameIdExtensions
    {
        /// <summary>
        /// Create XElement for the Saml2NameIdentifier.
        /// </summary>
        /// <param name="nameIdentifier"></param>
        /// <returns></returns>
        public static XElement ToXElement(this Saml2NameIdentifier nameIdentifier)
        {
            if(nameIdentifier == null)
            {
                throw new ArgumentNullException(nameof(nameIdentifier));
            }

            var nameIdElement = new XElement(Saml2Namespaces.Saml2 + "NameID",
                            nameIdentifier.Value);
            nameIdElement.AddAttributeIfNotNullOrEmpty("Format", nameIdentifier.Format);
            nameIdElement.AddAttributeIfNotNullOrEmpty("NameQualifier", nameIdentifier.NameQualifier);
            nameIdElement.AddAttributeIfNotNullOrEmpty("SPNameQualifier", nameIdentifier.SPNameQualifier);
            nameIdElement.AddAttributeIfNotNullOrEmpty("SPProvidedID", nameIdentifier.SPProvidedId);

            return nameIdElement;
        }
    }
}
