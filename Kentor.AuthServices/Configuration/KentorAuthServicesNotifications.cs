using System.Collections.Generic;
using Kentor.AuthServices.Saml2P;
using System;
using Kentor.AuthServices.WebSso;

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
            SignInCommandResultCreated = (cr, r) => { };
        }

        /// <summary>
        /// Notification called when a <see cref="Saml2AuthenticationRequest"/>
        /// has been created. The authenticationrequest can be amended and
        /// modified.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Action<Saml2AuthenticationRequest, IdentityProvider, IDictionary<string, string>> 
            AuthenticationRequestCreated { get; set; }

        /// <summary>
        /// Notification called when the SignIn command has produced a
        /// <see cref="CommandResult"/>, but before anything has been applied
        /// to the outgoing response. Set the <see cref="CommandResult.HandledResult"/>
        /// flag to suppress the libraries built in apply functionality to the
        /// outgoing response.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Action<CommandResult, IDictionary<string, string>>
            SignInCommandResultCreated { get; set; }
    }
}
