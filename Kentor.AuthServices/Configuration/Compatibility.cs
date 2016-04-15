using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Compatibility settings. Can be used to make AuthServices accept
    /// certain non-standard behaviour.
    /// </summary>
    public class Compatibility
    {
        /// <summary>
        /// If an EntitiesDescriptor element is found when loading metadata
        /// for en IdentityProvider, automatically check inside it if there
        /// is a single EntityDescriptor and in that case use it.
        /// </summary>
        public bool UnpackEntitiesDescriptorInIdentityProviderMetadata { get; set; }
    }
}
