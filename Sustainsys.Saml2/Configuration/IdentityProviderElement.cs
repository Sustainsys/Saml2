using Sustainsys.Saml2.WebSso;
using System;
using System.Configuration;
using Sustainsys.Saml2.Saml2P;

namespace Sustainsys.Saml2.Configuration
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

        const string signOnUrl = nameof(signOnUrl);
        /// <summary>
        /// Destination url to send sign in requests to.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "SignOn")]
        [ConfigurationProperty(signOnUrl)]
        public Uri SignOnUrl
        {
            get
            {
                return (Uri)base[signOnUrl];
            }
            internal set
            {
                base[signOnUrl] = value;
            }
        }

        const string logoutUrl = nameof(logoutUrl);
        /// <summary>
        /// Single logout url endpoint of Idp.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout")]
        [ConfigurationProperty(logoutUrl)]
        public Uri LogoutUrl
        {
            get
            {
                return (Uri)base[logoutUrl];
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

        const string outboundSigningAlgorithm = nameof(outboundSigningAlgorithm);
        /// <summary>
        /// Signing algorithm for outbound messages to this Idp. Overrides the
        /// main signature algorithm configured in <see cref="SPOptions"/>.
        /// </summary>
        [ConfigurationProperty(outboundSigningAlgorithm, IsRequired = false)]
        public string OutboundSigningAlgorithm
        {
            get
            {
                return (string)base[outboundSigningAlgorithm];
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

        const string metadataLocation = nameof(metadataLocation);

        /// <summary>
        /// Metadata location url to be used for automatic downloading of metadata.
        /// </summary>
        [ConfigurationProperty(metadataLocation)]
        public string MetadataLocation
        {
            get
            {
                return (string)base[metadataLocation];
            }
            internal set
            {
                base[metadataLocation] = value;
            }
        }

        const string artifactResolutionServices = nameof(artifactResolutionServices);
        /// <summary>
        /// Artifact Resolution endpoints for the identity provider.
        /// </summary>
        [ConfigurationProperty(artifactResolutionServices)]
        [ConfigurationCollection(typeof(ArtifactResolutionServiceCollection))]
        public ArtifactResolutionServiceCollection ArtifactResolutionServices
        {
            get
            {
                return (ArtifactResolutionServiceCollection)base[artifactResolutionServices];
            }
        }

        const string wantAuthnRequestsSigned = nameof(wantAuthnRequestsSigned);
        /// <summary>
        /// Does this Idp want the AuthnRequests to be signed?
        /// </summary>
        [ConfigurationProperty(wantAuthnRequestsSigned, IsRequired = false, DefaultValue = false)]
        public bool WantAuthnRequestsSigned
        {
            get
            {
                return (bool)base[wantAuthnRequestsSigned];
            }
        }

        const string disableOutboundLogoutRequests = nameof(disableOutboundLogoutRequests);

        /// <summary>
        /// Disable outbound logout requests to this idp, even though
        /// Saml2 is configured for single logout and the idp supports
        /// it. This setting might be usable when adding SLO to an existing
        /// setup, to ensure that everyone is ready for SLO before activating.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout")]
        [ConfigurationProperty(disableOutboundLogoutRequests, IsRequired = false, DefaultValue = false)]
        public bool DisableOutboundLogoutRequests
        {
            get
            {
                return (bool)base[disableOutboundLogoutRequests];
            }
            set
            {
                base[disableOutboundLogoutRequests] = value;
            }
        }
    }
}
