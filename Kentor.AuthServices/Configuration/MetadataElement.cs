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
    }
}
