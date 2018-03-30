using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Sustainsys.Saml2.Configuration
{
    /// <summary>
    /// Options implementation for handling in memory options.
    /// </summary>
    public class Options : IOptions
    {
        /// <summary>
        /// Set of callbacks that can be used as extension points for various
        /// events.
        /// </summary>
        public Saml2Notifications Notifications { get; set; }

        /// <summary>
        /// Reads the options from the current config file.
        /// </summary>
        /// <returns>Options object.</returns>
        public static Options FromConfiguration
        {
            get
            {
                return optionsFromConfiguration.Value;
            }
        }

        private static readonly Lazy<Options> optionsFromConfiguration 
            = new Lazy<Options>(() => LoadOptionsFromConfiguration(), true);

        private static Options LoadOptionsFromConfiguration()
        {
            var spOptions = new SPOptions(SustainsysSaml2Section.Current);
            var options = new Options(spOptions);
            SustainsysSaml2Section.Current.IdentityProviders.RegisterIdentityProviders(options);
            SustainsysSaml2Section.Current.Federations.RegisterFederations(options);

            return options;
        }

        /// <summary>
        /// Creates an options object with the specified SPOptions.
        /// </summary>
        /// <param name="spOptions"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public Options(SPOptions spOptions)
        {
            Notifications = new Saml2Notifications();
            SPOptions = spOptions;
            if(SPOptions.Logger == null)
            {
                SPOptions.Logger = new NullLoggerAdapter();
            }
        }

        /// <summary>
        /// Options for the service provider's behaviour; i.e. everything except
        /// the idp and federation list.
        /// </summary>
        public SPOptions SPOptions { get; }

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
    }
}
