using System;
using Kentor.AuthServices.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;

namespace Kentor.AuthServices.AspNetCore
{
    /// <summary>
    /// Options for Kentor AuthServices Saml2 Authentication.
    /// </summary>
    public class KentorAuthServicesOptions : AuthenticationOptions, IOptions
    {
        /// <summary>
        /// Set of callbacks that can be used as extension points for various
        /// events.
        /// </summary>
        public KentorAuthServicesNotifications Notifications { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public KentorAuthServicesOptions()
            : this(false)
        { }

        /// <summary>
        /// Constructor
        /// <param name="loadConfiguration">Should the options be inited by loading app/web.config?</param>
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "KentorAuthServices")]
        public KentorAuthServicesOptions(bool loadConfiguration)
            : base()
        {
            AutomaticAuthenticate = true;
            AuthenticationScheme = KentorAuthServicesDefaults.DefaultAuthenticationScheme;
            DisplayName = KentorAuthServicesDefaults.DefaultDisplayName;
            Notifications = new KentorAuthServicesNotifications();

            if(loadConfiguration)
            {
                SPOptions = new SPOptions(KentorAuthServicesSection.Current);
                KentorAuthServicesSection.Current.IdentityProviders.RegisterIdentityProviders(this);
                KentorAuthServicesSection.Current.Federations.RegisterFederations(this);
            }
        }

        /// <summary>
        /// The authentication type that will be used to sign in with. Typically this will be "ExternalCookie"
        /// to be picked up by the external cookie authentication middleware that persists the identity in a cookie.
        /// </summary>
        public string SignInAsAuthenticationType { get; set; }

        public string AugmentLogoutAuthenticationType { get; set; }

        /// <summary>
        /// Options for the service provider's behaviour; i.e. everything except
        /// the idp and federation list.
        /// </summary>
        public SPOptions SPOptions { get; set; }

        private readonly IdentityProviderDictionary identityProviders = new IdentityProviderDictionary();

        /// <summary>
        /// Available identity providers.
        /// </summary>
        public IdentityProviderDictionary IdentityProviders
        {
            get
            {
                return identityProviders;
            }
        }

        /// <summary>
        /// Passthrough property to Description.DisplayName.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return Description.DisplayName;
            }
            set
            {
                Description.DisplayName = value;
            }
        }

        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        internal IDataProtector DataProtector { get; set; }
    }
}
