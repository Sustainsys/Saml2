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
        /// Ctor
        /// </summary>
        public Compatibility()
        { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="configElement">Config element to load</param>
        public Compatibility(CompatibilityElement configElement)
        {
            if(configElement == null)
            {
                throw new ArgumentNullException(nameof(configElement));
            }

            UnpackEntitiesDescriptorInIdentityProviderMetadata =
                configElement.UnpackEntitiesDescriptorInIdentityProviderMetadata;
            DisableLogoutStateCookie = configElement.DisableLogoutStateCookie;
        }

        /// <summary>
        /// If an EntitiesDescriptor element is found when loading metadata
        /// for an IdentityProvider, automatically check inside it if there
        /// is a single EntityDescriptor and in that case use it.
        /// </summary>
        public bool UnpackEntitiesDescriptorInIdentityProviderMetadata { get; set; }

        /// <summary>
        /// Do not send logout state cookie, e.g. if you are not using ReturnUrl
        /// or if you know the cookie will be lost due to cross-domain redirects
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout" )]
        public bool DisableLogoutStateCookie { get; set; }

        /// <summary>
        /// Honor the owin authentication mode even on logout. Normally the logout
        /// handling is always done as if the middleware was active, to allow for
        /// simple sign out without specifying an auth type.
        /// </summary>
        public bool StrictOwinAuthenticationMode { get; set; }
    }
}
