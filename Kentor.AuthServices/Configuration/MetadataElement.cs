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
        /// Optional attribute that describes for how long in seconds anyone may cache the metadata 
        /// presented by the service provider. Defaults to 3600 seconds.
        /// </summary>
        [ConfigurationProperty(cacheDuration, IsRequired = false, DefaultValue = "1:0:0")]
        public TimeSpan CacheDuration
        {
            get
            {
                return (TimeSpan)base[cacheDuration];
            }
        }
    }
}
