using System.Configuration;
using Kentor.AuthServices.Saml2P;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// NamedId policy configuration element.
    /// </summary>
    public class NameIdPolicyElement : ConfigurationElement
    {
        /// <summary>
        /// Allow create.
        /// </summary>
        [ConfigurationProperty("allowCreate", IsRequired = false, DefaultValue = "false")]
        public bool AllowCreate
        {
            get
            {
                return (bool)base["allowCreate"];
            }
        }

        /// <summary>
        /// The NameId format.
        /// </summary>
        [ConfigurationProperty("format", IsRequired = false, DefaultValue = NameIdFormat.Transient)]
        public NameIdFormat Format
        {
            get
            {
                return (NameIdFormat)base["format"];
            }
        }
    }
}