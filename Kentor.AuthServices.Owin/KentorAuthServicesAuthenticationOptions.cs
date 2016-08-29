using Kentor.AuthServices.Configuration;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Configuration;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Owin
{
    /// <summary>
    /// Options for Kentor AuthServices Saml2 Authentication.
    /// </summary>
    public class KentorAuthServicesAuthenticationOptions : AuthenticationOptions, IOptions
    {
        /// <summary>
        /// Set of callbacks that can be used as extension points for various
        /// events.
        /// </summary>
        public KentorAuthServicesNotifications Notifications { get; set; }

        /// <summary>
        /// Constructor
        /// <param name="loadConfiguration">Should the options be inited by loading app/web.config?</param>
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "KentorAuthServices")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Owin.Security.AuthenticationDescription.set_Caption(System.String)")]
        public KentorAuthServicesAuthenticationOptions(bool loadConfiguration)
            : base(Constants.DefaultAuthenticationType)
        {
            AuthenticationMode = AuthenticationMode.Active;
            Description.Caption = Constants.DefaultCaption;
            Notifications = new KentorAuthServicesNotifications();

            if (loadConfiguration)
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
        /// Passthrough property to Description.Caption.
        /// </summary>
        public string Caption
        {
            get
            {
                return Description.Caption;
            }
            set
            {
                Description.Caption = value;
            }
        }

        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        internal IDataProtector DataProtector { get; set; }
    }
}
