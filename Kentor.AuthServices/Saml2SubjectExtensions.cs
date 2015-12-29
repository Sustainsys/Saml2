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
            if(subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            return new XElement(Saml2Namespaces.Saml2 + "Subject",
                    new XElement(Saml2Namespaces.Saml2 + "NameID",
                    subject.NameId.Value),
                    new XElement(Saml2Namespaces.Saml2 + "SubjectConfirmation",
                        new XAttribute("Method", "urn:oasis:names:tc:SAML:2.0:cm:bearer"))
                    );
        }
    }
}
