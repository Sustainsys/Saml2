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
    public class EntityDescriptorExtensions
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
