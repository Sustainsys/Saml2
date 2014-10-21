using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Contact person for a SAML2 entity.
    /// </summary>
    [DebuggerDisplay("ContactPersonElement {ContactType} {Email}")]
    public class ContactPersonElement : ConfigurationElement
    {
        const string contactType = "type";

        /// <summary>
        /// The type of this contact. A value from the 
        /// System.IdentityModel.Metadata.ContactType enumeration.
        /// </summary>
        [ConfigurationProperty(contactType, IsRequired = true)]
        public ContactType ContactType
        {
            get
            {
                return (ContactType)base[contactType];
            }
        }

        const string company = "company";

        /// <summary>
        /// Name of the company of the contact.
        /// </summary>
        [ConfigurationProperty(company)]
        public string Company
        {
            get
            {
                return (string)base[company];
            }
        }

        const string givenName = "givenName";

        /// <summary>
        /// Given name of the contact.
        /// </summary>
        [ConfigurationProperty(givenName)]
        public string GivenName
        {
            get
            {
                return (string)base[givenName];
            }
        }

        const string surname = "surname";

        /// <summary>
        /// Surname of the contact.
        /// </summary>
        [ConfigurationProperty(surname)]
        public string Surname
        {
            get
            {
                return (string)base[surname];
            }
        }

        const string phoneNumber = "phoneNumber";

        /// <summary>
        /// Phone number of the contact.
        /// </summary>
        [ConfigurationProperty(phoneNumber)]
        public string PhoneNumber
        {
            get
            {
                return (string)base[phoneNumber];
            }
        }

        const string email = "email";

        /// <summary>
        /// E-mail of the contact.
        /// </summary>
        [ConfigurationProperty(email)]
        public string Email
        {
            get
            {
                return (string)base[email];
            }
        }

    }
}
