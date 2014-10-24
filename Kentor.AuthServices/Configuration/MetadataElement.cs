using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Metadata configuration.
    /// </summary>
    public class MetadataElement : ConfigurationElement
    {
        const string organization = "organization";

        /// <summary>
        /// Information about organization.
        /// </summary>
        [ConfigurationProperty(organization)]
        public OrganizationElement Organization
        {
            get
            {
                return (OrganizationElement)base[organization];
            }
        }

        const string cacheDuration = "cacheDuration";
        /// <summary>
        /// Optional attribute that describes for how long anyone may cache the metadata
        /// presented by the service provider. Defaults to 1 hour.
        /// </summary>
        [ConfigurationProperty(cacheDuration, IsRequired = false, DefaultValue = "1:0:0")]
        public TimeSpan CacheDuration
        {
            get
            {
                return (TimeSpan)base[cacheDuration];
            }
        }

        /// <summary>
        /// Collection of contacts.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection=true)]
        [ConfigurationCollection(typeof(ContactPersonsCollection), AddItemName="contactPerson")]
        public ContactPersonsCollection Contacts
        {
            get
            {
                return (ContactPersonsCollection)base[""];
            }
        }

        const string requestedAttributes = "requestedAttributes";

        /// <summary>
        /// Requested attributes of the service provider.
        /// </summary>
        [ConfigurationProperty(requestedAttributes)]
        [ConfigurationCollection(typeof(RequestedAttributesCollection))]
        public RequestedAttributesCollection RequestedAttributes
        {
            get
            {
                return(RequestedAttributesCollection)base[requestedAttributes];
            }
        }
    }
}
