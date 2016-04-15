using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Compatibility settings. Can be used to make AuthServices accept
    /// certain non-standard behaviour.
    /// </summary>
    public class CompatibilityElement : ConfigurationElement
    {
        internal bool AllowChange { get; set; }

        /// <summary>
        /// Used for testing, always returns true in production.
        /// </summary>
        /// <returns>Returns true (unless during tests)</returns>
        public override bool IsReadOnly()
        {
            return !AllowChange;
        }

        const string unpackEntitiesDescriptorInIdentityProviderMetadata
            = nameof(unpackEntitiesDescriptorInIdentityProviderMetadata);

        /// <summary>
        /// If an EntitiesDescriptor element is found when loading metadata
        /// for an IdentityProvider, automatically check inside it if there
        /// is a single EntityDescriptor and in that case use it.
        /// </summary>
        [ConfigurationProperty(nameof(unpackEntitiesDescriptorInIdentityProviderMetadata), IsRequired = false)]
        public bool UnpackEntitiesDescriptorInIdentityProviderMetadata
        {
            get
            {
                return (bool)base[unpackEntitiesDescriptorInIdentityProviderMetadata];
            }
            set
            {
                base[unpackEntitiesDescriptorInIdentityProviderMetadata] = value;
            }
        }
    }
}
