using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.Saml2P;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Options for the service provider's behaviour; i.e. everything except
    /// the idp and federation list.
    /// </summary>
    public interface ISPOptions
    {
        /// <summary>
        /// Return Uri to redirect the client to, if no return uri was specified
        /// when initiating the signin sequence.
        /// </summary>
        Uri ReturnUrl { get; }

        /// <summary>
        /// Recommendation of cache refresh interval to those who reads our
        /// metadata.
        /// </summary>
        TimeSpan MetadataCacheDuration { get; }

        /// <summary>
        /// Maximum validity duration after fetch for those who reads our
        /// metadata. Exposed as an absolute validUntil time in the metadata.
        /// If set to null, no validUntil is exposed in metadata.
        /// </summary>
        TimeSpan? MetadataValidDuration { get; }

        /// <summary>
        /// The security token handler used to process incoming assertions for this SP.
        /// </summary>
        Saml2PSecurityTokenHandler Saml2PSecurityTokenHandler { get; }

        /// <summary>
        /// Url to discovery service to use if no idp is specified in the sign in call.
        /// </summary>
        Uri DiscoveryServiceUrl { get; }

        /// <summary>
        /// EntityId - The identity of the ServiceProvider to use when sending requests to Idp
        /// and presenting the SP in metadata.
        /// </summary>
        EntityId EntityId { get; }

        /// <summary>
        /// Application root relative path for AuthServices endpoints. The
        /// default should be "/AuthServices".
        /// </summary>
        string ModulePath { get; }

        /// <summary>
        /// By default, the service provider uses the host, protocol, port and
        /// application root path from the HTTP request when creating links. 
        /// This might not be accurate in reverse proxy or load-balancing
        /// situations. You can override the origin used for link generation
        /// using this property.
        /// </summary>
        Uri PublicOrigin { get; }

        /// <summary>
        /// Metadata describing the organization responsible for the SAML2 entity.
        /// </summary>
        Organization Organization { get; }

        /// <summary>
        /// NameId Policy.
        /// </summary>
        Saml2NameIdPolicy NameIdPolicy { get; }

        /// <summary>
        /// RequestedAuthnContext
        /// </summary>
        Saml2RequestedAuthnContext RequestedAuthnContext { get;  }

        /// <summary>
        /// Contacts for the SAML2 entity. Must not be null.
        /// </summary>
        IEnumerable<ContactPerson> Contacts { get; }

        /// <summary>
        /// Attribute consuming services for the service provider.
        /// </summary>
        IEnumerable<AttributeConsumingService> AttributeConsumingServices { get; }

        /// <summary>
        /// The System.IdentityModel configuration to use.
        /// </summary>
        IdentityConfiguration SystemIdentityModelIdentityConfiguration { get; }

        /// <summary>
        /// Certificates used by the service provider for signing or decryption.
        /// </summary>
        ICollection<ServiceCertificate> ServiceCertificates { get; }

        /// <summary>
        /// Certificates valid for use in decryption
        /// </summary>
        ReadOnlyCollection<X509Certificate2> DecryptionServiceCertificates { get; }

        /// <summary>
        /// Certificate for use in signing outbound requests
        /// </summary>
        X509Certificate2 SigningServiceCertificate { get; }

        /// <summary>
        /// Certificates to be published in metadata
        /// </summary>
        ReadOnlyCollection<ServiceCertificate> MetadataCertificates { get; }

        /// <summary>
        /// Signing behavior for AuthnRequests.
        /// </summary>
        SigningBehavior AuthenticateRequestSigningBehavior { get; }

        /// <summary>
        /// Metadata flag that we want assertions to be signed.
        /// </summary>
        bool WantAssertionsSigned { get; }

        /// <summary>
        /// Validate certificates when validating signatures? Normally not a
        /// good idea as SAML2 deployments typically exchange certificates
        /// directly and isntead of relying on the public certificate
        /// infrastructure.
        /// </summary>
        bool ValidateCertificates { get; }
    }
}
