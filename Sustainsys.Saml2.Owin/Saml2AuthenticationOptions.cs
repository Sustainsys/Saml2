using Sustainsys.Saml2.Configuration;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Infrastructure;

namespace Sustainsys.Saml2.Owin
{
    /// <summary>
    /// Options for Sustainsys Saml2 OWIN Authentication.
    /// </summary>
    public class Saml2AuthenticationOptions : AuthenticationOptions, IOptions
    {
        /// <summary>
        /// Set of callbacks that can be used as extension points for various
        /// events.
        /// </summary>
        public Saml2Notifications Notifications { get; set; }

        /// <summary>
        /// Constructor
        /// <param name="loadConfiguration">Should the options be inited by loading app/web.config?</param>
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SustainsysSaml2")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Owin.Security.AuthenticationDescription.set_Caption(System.String)")]
        public Saml2AuthenticationOptions(bool loadConfiguration)
            : base(Constants.DefaultAuthenticationType)
        {
            AuthenticationMode = AuthenticationMode.Passive;
            Description.Caption = Constants.DefaultCaption;
            Notifications = new Saml2Notifications();
            CookieManager = new CookieManager();

            if (loadConfiguration)
            {
                SPOptions = new SPOptions(SustainsysSaml2Section.Current);
                SustainsysSaml2Section.Current.IdentityProviders.RegisterIdentityProviders(this);
                SustainsysSaml2Section.Current.Federations.RegisterFederations(this);
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
        /// Gets or sets the cookie manager used for reading and writing cookies
        /// </summary>
        public ICookieManager CookieManager { get; set; }

        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        internal IDataProtector DataProtector { get; set; }
    }
}
