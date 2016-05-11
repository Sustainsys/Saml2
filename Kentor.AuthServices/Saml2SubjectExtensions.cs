using System;
using System.IdentityModel.Tokens;
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
            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            var element = new XElement(Saml2Namespaces.Saml2 + "Subject",
                subject.NameId.ToXElement());

            if (subject.SubjectConfirmations != null)
            {
                foreach (var subjectConfirmation in subject.SubjectConfirmations)
                {
                    element.Add(subjectConfirmation.ToXElement());
                }
            }

            return element;
        }

        /// <summary>
        /// Writes out the subject confirmation as an XElement.
        /// </summary>
        /// <param name="subjectConfirmation"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static XElement ToXElement(this Saml2SubjectConfirmation subjectConfirmation)
        {
            if (subjectConfirmation == null)
            {
                throw new ArgumentNullException(nameof(subjectConfirmation));
            }

            var element = new XElement(Saml2Namespaces.Saml2 + "SubjectConfirmation",
                new XAttribute("Method", subjectConfirmation.Method.OriginalString));

            if (subjectConfirmation.SubjectConfirmationData != null)
            {
                element.Add(subjectConfirmation.SubjectConfirmationData.ToXElement());
            }

            return element;
        }

        /// <summary>
        /// Writes out the subject confirmation data as an XElement.
        /// </summary>
        /// <param name="subjectConfirmationData"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static XElement ToXElement(this Saml2SubjectConfirmationData subjectConfirmationData)
        {
            if (subjectConfirmationData == null)
            {
                throw new ArgumentNullException(nameof(subjectConfirmationData));
            }

            var element = new XElement(Saml2Namespaces.Saml2 + "SubjectConfirmationData");

            if (subjectConfirmationData.NotOnOrAfter.HasValue)
            {
                element.SetAttributeValue("NotOnOrAfter", 
                    subjectConfirmationData.NotOnOrAfter.Value.ToSaml2DateTimeString());
            }

            if (subjectConfirmationData.InResponseTo != null)
            {
                element.SetAttributeValue("InResponseTo", subjectConfirmationData.InResponseTo.Value);
            }

            if (subjectConfirmationData.Recipient != null)
            {
                element.SetAttributeValue("Recipient", subjectConfirmationData.Recipient.OriginalString);
            }

            if (subjectConfirmationData.NotBefore.HasValue)
            {
                element.SetAttributeValue("NotBefore",
                    subjectConfirmationData.NotBefore.Value.ToSaml2DateTimeString());
            }

            return element;
        }
    }
}
