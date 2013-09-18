using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config element for the identity provider element.
    /// </summary>
    public class IdentityProviderElement : ConfigurationElement
    {
        /// <summary>
        /// Name of the idp in the configuration.
        /// </summary>
        [ConfigurationProperty("name", IsRequired=true)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }

        /// <summary>
        /// Destination url to send requests to.
        /// </summary>
        [ConfigurationProperty("destinationUri", IsRequired=true)]
        public Uri DestinationUri
        {
            get
            {
                return (Uri)base["destinationUri"];
            }
        }

        /// <summary>
        /// The binding to use when sending requests to the Idp.
        /// </summary>
        [ConfigurationProperty("binding", IsRequired=true)]
        public Saml2BindingType Binding
        {
            get
            {
                return (Saml2BindingType)base["binding"];
            }
        }
    }
}
