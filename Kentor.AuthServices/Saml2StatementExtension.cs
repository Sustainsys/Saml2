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
        public static XElement ToXElement(this Saml2Statement statement)
        {
            return statement.ToXElement(false);
        }

        /// <summary>
        /// Writes out the statement as an XElement.
        /// </summary>
        /// <param name="statement">Statement to create xml for.</param>
        /// <param name="enforceXmlns">Enforce xmlns values for attributes.</param>
        /// <returns>XElement</returns>
        public static XElement ToXElement(this Saml2Statement statement, bool enforceXmlns)
        {
            if (statement == null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            var attributeStatement = statement as Saml2AttributeStatement;
            if (attributeStatement != null)
            {
                return ToXElement(attributeStatement, enforceXmlns);
            }

            var authnStatement = statement as Saml2AuthenticationStatement;
            if (authnStatement != null)
            {
                return ToXElement(authnStatement);
            }

            throw new NotImplementedException("Statement of type " + statement.GetType().Name + " is not supported.");
        }

        private static XElement ToXElement(Saml2AuthenticationStatement authnStatement)
        {
            var result = new XElement(Saml2Namespaces.Saml2 + "AuthnStatement",
                new XAttribute("AuthnInstant", authnStatement.AuthenticationInstant.ToSaml2DateTimeString()),
                new XElement(Saml2Namespaces.Saml2 + "AuthnContext",
                    new XElement(Saml2Namespaces.Saml2 + "AuthnContextClassRef",
                        authnStatement.AuthenticationContext.ClassReference.OriginalString)));

            if (authnStatement.SessionIndex != null)
            {
                result.Add(new XAttribute("SessionIndex", authnStatement.SessionIndex));
            }

            return result;
        }

        private static XElement ToXElement(Saml2AttributeStatement attributeStatement, bool enforceXmlns)
        {
            var element = new XElement(Saml2Namespaces.Saml2 + "AttributeStatement");

            foreach (var attribute in attributeStatement.Attributes)
            {
                var attributeElement = new XElement(Saml2Namespaces.Saml2 + "Attribute", new XAttribute("Name", attribute.Name));

                attributeElement.AddAttributeIfNotNullOrEmpty("FriendlyName", attribute.FriendlyName);
                attributeElement.AddAttributeIfNotNullOrEmpty("NameFormat", attribute.NameFormat);
                attributeElement.AddAttributeIfNotNullOrEmpty("OriginalIssuer", attribute.OriginalIssuer);

                XNamespace xs = @"http://www.w3.org/2001/XMLSchema";
                XNamespace xsi = @"http://www.w3.org/2001/XMLSchema-instance";

                foreach (var value in attribute.Values)
                {
                    if (enforceXmlns)
                        attributeElement.Add(new XElement(Saml2Namespaces.Saml2 + "AttributeValue", value,
                            new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                            new XAttribute(XNamespace.Xmlns + "xs", xs.NamespaceName),
                            new XAttribute(xsi + "type", @"xs:string")));
                    else
                        attributeElement.Add(new XElement(Saml2Namespaces.Saml2 + "AttributeValue", value));
                }

                element.Add(attributeElement);
            }

            return element;
        }
    }
}
