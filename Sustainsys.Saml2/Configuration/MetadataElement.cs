using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using Sustainsys.Saml2.Metadata;

namespace Sustainsys.Saml2.Configuration
{
	public class XsdDurationConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return new XsdDuration((string)value);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return ((XsdDuration)value).ToString();
		}
	}

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
        [ConfigurationProperty(cacheDuration, IsRequired = false, DefaultValue = "PT1H")]
		[TypeConverter(typeof(XsdDurationConverter))]
        public XsdDuration CacheDuration
        {
            get
            {
                return (XsdDuration)base[cacheDuration];
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
