using System.Collections.Generic;
using Kentor.AuthServices.Saml2P;
using System;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Set of callbacks that can be used as extension points for various
    /// events.
    /// </summary>
    public class KentorAuthServicesNotifications
    {
        /// <summary>
        /// Ctor, setting all callbacks to do-nothing versions.
        /// </summary>
        public KentorAuthServicesNotifications()
        {
            AuthenticationRequestCreated = (request, provider, dictionary) => { };
        }

        /// <summary>
        /// Notification called when an <see cref="Saml2AuthenticationRequest"/>
        /// has been created. The authenticationrequest can be amended and
        /// modified.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Action<Saml2AuthenticationRequest, IdentityProvider, IDictionary<string, string>> 
            AuthenticationRequestCreated { get; set; }
    }
}
