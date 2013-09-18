using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices.Configuration
{
    public class IdentityProviderElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired=true)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }

        [ConfigurationProperty("destinationUri", IsRequired=true)]
        public Uri DestinationUri
        {
            get
            {
                return (Uri)base["destinationUri"];
            }
        }

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
