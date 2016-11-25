using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config for a requested element in the SPs metadata.
    /// </summary>
    public class RequestedAttributeElement : ConfigurationElement
    {
        const string name = "name";

        /// <summary>
        /// Name of the attribute. Usually on the form urn:oid:....
        /// </summary>
        [ConfigurationProperty(name, IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)base[name];
            }
        }

        const string friendlyName = "friendlyName";

        /// <summary>
        /// Friendly, human readable name of the attribute.
        /// </summary>
        [ConfigurationProperty(friendlyName)]
        public string FriendlyName
        {
            get
            {
                return (string)base[friendlyName];
            }
        }

        const string nameFormat = "nameFormat";

        /// <summary>
        /// Format of the Name property, one of the standard Uris in the saml specification.
        /// </summary>
        [ConfigurationProperty(nameFormat, DefaultValue="urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified")]
        public Uri NameFormat
        {
            get
            {
                return (Uri)base[nameFormat];
            }
        }

        const string isRequired = "isRequired";

        /// <summary>
        /// Is this attribute required by the SP for it to work correctly?
        /// </summary>
        [ConfigurationProperty(isRequired)]
        public bool IsRequired
        {
            get
            {
                return (bool)base[isRequired];
            }
        }
    }
}
