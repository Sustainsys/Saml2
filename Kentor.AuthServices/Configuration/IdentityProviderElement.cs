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
        /// Issuer as presented by the idp. Used as key to configuration.
        /// </summary>
        [ConfigurationProperty("issuer", IsRequired = true)]
        public string Issuer
        {
            get
            {
                return (string)base["issuer"];
            }
        }

        /// <summary>
        /// Destination url to send requests to.
        /// </summary>
        [ConfigurationProperty("destinationUri", IsRequired = true)]
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
        [ConfigurationProperty("binding", IsRequired = true)]
        public Saml2BindingType Binding
        {
            get
            {
                return (Saml2BindingType)base["binding"];
            }
        }

        /// <summary>
        /// File to load the signing certificate from.
        /// </summary>
        [ConfigurationProperty("signingCertificateFile")]
        public string SigningCertificateFile
        {
            get
            {
                return (string)base["signingCertificateFile"];
            }
        }
    }
}
