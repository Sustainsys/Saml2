using Kentor.AuthServices.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;

namespace Sustainsys.Saml2.AspNetCore2
{
    /// <summary>
    /// Options for Saml2 Authentication
    /// </summary>
    public class Saml2Options : RemoteAuthenticationOptions, IOptions
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public Saml2Options()
        {
            SPOptions = new SPOptions()
            {
                ModulePath = "/Saml2"
            };
            CallbackPath = SPOptions.ModulePath + "/Acs";
        }

        /// <summary>
        /// Options for the service provider's behaviour; i.e. everything except
        /// the idp list and the notifications.
        /// </summary>
        public SPOptions SPOptions { get; set; }

        /// <summary>
        /// Information about known identity providers.
        /// </summary>
        public IdentityProviderDictionary IdentityProviders { get; }
            = new IdentityProviderDictionary();

        /// <summary>
        /// Set of callbacks that can be used as extension points for various
        /// events.
        /// </summary>
        public KentorAuthServicesNotifications Notifications =>
            new KentorAuthServicesNotifications();
    }
}