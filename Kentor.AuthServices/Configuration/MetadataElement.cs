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
        /// <summary>
        /// Used by tests to write-enable config.
        /// </summary>
        internal bool AllowChange { get; set; }

        /// <summary>
        /// Is the element contents read only? Always true in production, but
        /// can be changed during tests.
        /// </summary>
        /// <returns>Is the element contents read only?</returns>
        public override bool IsReadOnly()
        {
            return !AllowChange;
        }

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

        const string cacheDuration = nameof(cacheDuration);
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

        const string validDuration = nameof(validDuration);
        /// <summary>
        /// How long after generation should the metadata be valid?
        /// </summary>
        [ConfigurationProperty(validDuration, IsRequired = false)]
        public TimeSpan? ValidUntil
        {
            get
            {
                return (TimeSpan?)base[validDuration];
            }
        }

        /// <summary>
        /// Collection of contacts.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ContactPersonsCollection), AddItemName = "contactPerson")]
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
                return (RequestedAttributesCollection)base[requestedAttributes];
            }
        }

        const string wantAssertionsSigned = nameof(wantAssertionsSigned);
        /// <summary>
        /// Metadata flag that we want assertions to be signed.
        /// </summary>
        [ConfigurationProperty(wantAssertionsSigned, IsRequired=false, DefaultValue=false)]
        public bool WantAssertionsSigned
        {
            get
            {
                return (bool)base[wantAssertionsSigned];
            }
            internal set
            {
                base[wantAssertionsSigned] = value;
            }
        }
    }
}
