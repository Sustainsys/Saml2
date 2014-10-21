using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Information about the organization responsible for the entity.
    /// </summary>
    public class OrganizationElement : ConfigurationElement
    {
        // Used from tests.
        internal void AllowConfigEdits(bool allow)
        {
            allowConfigEdits = allow;
        }

        private bool allowConfigEdits = false;

        /// <summary>
        /// Is this section readonly?
        /// </summary>
        /// <returns>Is this section readonly?</returns>
        public override bool IsReadOnly()
        {
            return !allowConfigEdits;
        }

        const string name = "name";

        /// <summary>
        /// Name of the organization.
        /// </summary>
        [ConfigurationProperty(name, IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)base[name];
            }
            set
            {
                base[name] = value;
            }
        }

        const string displayName = "displayName";

        /// <summary>
        /// Display name of the organization.
        /// </summary>
        [ConfigurationProperty(displayName, IsRequired = true)]
        public string DisplayName
        {
            get
            {
                return (string)base[displayName];
            }
            set
            {
                base[displayName] = value;
            }
        }

        const string url = "url";

        /// <summary>
        /// Url of the organization.
        /// </summary>
        [ConfigurationProperty(url, IsRequired = true)]
        public Uri Url
        {
            get
            {
                return (Uri)base[url];
            }
            set
            {
                base[url] = value;
            }
        }

        const string language = "language";

        /// <summary>
        /// The language that should be used for the organization strings.
        /// </summary>
        [ConfigurationProperty(language)]
        public string Language
        {
            get
            {
                return (string)base[language];
            }
        }
    }
}
