using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
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
        Uri ReturnUri { get; }

        /// <summary>
        /// Optional attribute that describes for how long in seconds anyone may cache the metadata
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
        /// Url where to receive discovery service responses.
        /// </summary>
        Uri DiscoveryServiceResponseUrl { get; }

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
    }
}
