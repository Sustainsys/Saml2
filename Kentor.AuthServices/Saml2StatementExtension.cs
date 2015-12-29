using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Kentor.AuthServices.Internal;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Extension methods for Saml2Statement
    /// </summary>
    public static class Saml2StatementExtension
    {
        /// <summary>
        /// Writes out the statement as an XElement.
        /// </summary>
        /// <param name="statement">Statement to create xml for.</param>
        /// <returns>XElement</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public static XElement ToXElement(this Saml2Statement statement)
        {
            if (statement == null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            if (statement is Saml2AttributeStatement)
            {
                var attributeStatement = statement as Saml2AttributeStatement;
                var element = new XElement(Saml2Namespaces.Saml2 + "AttributeStatement");

                foreach (var attribute in attributeStatement.Attributes)
                {
                    var attributeElement = new XElement(Saml2Namespaces.Saml2 + "Attribute", new XAttribute("Name", attribute.Name));

                    attributeElement.AddAttributeIfNotNullOrEmpty("FriendlyName", attribute.FriendlyName);
                    attributeElement.AddAttributeIfNotNullOrEmpty("NameFormat", attribute.NameFormat);
                    attributeElement.AddAttributeIfNotNullOrEmpty("OriginalIssuer", attribute.OriginalIssuer);

                    foreach(var value in attribute.Values)
                    {
                        attributeElement.Add(new XElement(Saml2Namespaces.Saml2 + "AttributeValue", value));
                    }

                    element.Add(attributeElement);
                }

                return element;
            }
            else
            {
                throw new ArgumentNullException(nameof(statement));
            }
        }
    }
}
