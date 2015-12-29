using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.Saml2P;
using System;
using System.Collections.Generic;
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
        /// Optional attribute that describes for how long anyone may cache the metadata
        /// presented by the service provider. Defaults to 3600 seconds.
        /// </summary>
        TimeSpan MetadataCacheDuration { get; }

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
        /// Metadata describing the organization responsible for the SAML2 entity.
        /// </summary>
        Organization Organization { get; }

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
        /// Certificate for service provider to use when decrypting assertions
        /// </summary>
        X509Certificate2 ServiceCertificate { get; set; }
    }
}
