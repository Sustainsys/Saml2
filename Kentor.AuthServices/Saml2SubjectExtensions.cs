using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Extension methods for Saml2Subject
    /// </summary>
    public static class Saml2SubjectExtensions
    {
        /// <summary>
        /// Writes out the subject as an XElement.
        /// </summary>
        /// <param name="subject">The subject to create xml for.</param>
        /// <returns>XElement</returns>
        public static XElement ToXElement(this Saml2Subject subject)
        {
            return ToXElement(subject, null, null);
        }

        /// <summary>
        /// Writes out the subject as an XElement.
        /// </summary>
        /// <param name="subject">The subject to create xml for.</param>
        /// <param name="destination">The destination to create xml for.</param>
        /// <param name="inResponseTo">The request ID.</param>
        /// <returns>XElement</returns>
        public static XElement ToXElement(this Saml2Subject subject, Uri destination, Saml2Id inResponseTo)
        {
            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            var confirmationData = new XElement(Saml2Namespaces.Saml2 + "SubjectConfirmationData",
                        new XAttribute("NotOnOrAfter",
                            DateTime.UtcNow.AddMinutes(2).ToSaml2DateTimeString()));
            if (destination != null)
            {
                confirmationData.SetAttributeValue("Recipient", destination.AbsoluteUri);
            }
            if (inResponseTo != null)
            {
                confirmationData.SetAttributeValue("InResponseTo", inResponseTo);
            }

            return new XElement(Saml2Namespaces.Saml2 + "Subject",
                subject.NameId.ToXElement(),
                new XElement(Saml2Namespaces.Saml2 + "SubjectConfirmation",
                    new XAttribute("Method", "urn:oasis:names:tc:SAML:2.0:cm:bearer"),
                    confirmationData)
                );
        }
    }
}
