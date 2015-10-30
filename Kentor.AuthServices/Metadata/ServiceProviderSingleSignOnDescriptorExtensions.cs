using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices.Metadata
{
    /// <summary>
    /// Metadata extensions
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "SignOn", Justification = "Using SAML2 established terms." )]
    public class ServiceProviderSingleSignOnDescriptorExtensions
    {
        /// <summary>
        /// Discovery Service response url.
        /// </summary>
        public IndexedProtocolEndpoint DiscoveryResponse
        {
            get;
            set;
        }
    }
}
