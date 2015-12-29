using Kentor.AuthServices.WebSso;
using System;
using System.Configuration;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config element for the identity provider element.
    /// </summary>
    public class IdentityProviderElement : ConfigurationElement
    {
        private bool isReadOnly = true;

        internal void AllowConfigEdit(bool allow)
        {
            isReadOnly = !allow;
        }

        /// <summary>
        /// Allows local modification of the configuration for testing purposes
        /// </summary>
        /// <returns></returns>
        public override bool IsReadOnly()
        {
            return isReadOnly;
        }
                
        /// <summary>
        /// EntityId as presented by the idp. Used as key to configuration.
        /// </summary>
        [ConfigurationProperty("entityId", IsRequired = true)]
        public string EntityId
        {
            get
            {
                return (string)base["entityId"];
            }
            internal set
            {
                base["entityId"] = value;
            }
        }

        /// <summary>
        /// Destination url to send requests to.
        /// </summary>
        [ConfigurationProperty("destinationUrl")]
        public Uri DestinationUrl
        {
            get
            {
                return (Uri)base["destinationUrl"];
            }
            internal set
            {
                base["destinationUrl"] = value;
            }
        }

        /// <summary>
        /// Destination url to send logout requests to.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout"), ConfigurationProperty("singleLogoutUrl")]
        public Uri SingleLogoutUrl
        {
            get
            {
                return (Uri)base["singleLogoutUrl"];
            }
            internal set
            {
                base["singleLogoutUrl"] = value;
            }
        }

        /// <summary>
        /// The binding to use when sending requests to the Idp.
        /// </summary>
        [ConfigurationProperty("binding")]
        public Saml2BindingType Binding
        {
            get
            {
                return (Saml2BindingType)base["binding"];
            }
            internal set
            {
                base["binding"] = value;
            }
        }

        /// <summary>
        /// Certificate location for the certificate the Idp uses to sign its messages.
        /// </summary>
        [ConfigurationProperty("signingCertificate")]
        public CertificateElement SigningCertificate
        {
            get
            {
                return (CertificateElement)base["signingCertificate"];
            }
            internal set
            {
                base["signingCertificate"] = value;
            }
        }

        /// <summary>
        /// Allow unsolicited responses. That is InResponseTo is missing in the AuthnRequest.
        /// If true InResponseTo is not required.
        /// If false InResponseTo is required.
        /// Even though AllowUnsolicitedAuthnResponse is true the InResponseTo must be valid if existing.
        /// </summary>
        [ConfigurationProperty("allowUnsolicitedAuthnResponse", IsRequired = true)]
        public bool AllowUnsolicitedAuthnResponse 
        {
            get
            {
                return (bool)base["allowUnsolicitedAuthnResponse"];
            }
        }

        /// <summary>
        /// Enable automatic downloading of metadata form the well-known uri (i.e. interpret
        /// the EntityID as an uri and download metadata from it).
        /// </summary>
        [ConfigurationProperty("loadMetadata", IsRequired = false, DefaultValue = false)]
        public bool LoadMetadata
        {
            get
            {
                return (bool)base["loadMetadata"];
            }
            set
            {
                base["loadMetadata"] = value;
            }
        }

        const string metadataUrl = "metadataUrl";

        /// <summary>
        /// Metadata location url to be used for automatic downloading of metadata.
        /// </summary>
        [ConfigurationProperty(metadataUrl)]
        public Uri MetadataUrl
        {
            get
            {
                return (Uri)base[metadataUrl];
            }
            internal set
            {
                base[metadataUrl] = value;
            }
        }
    }
}
