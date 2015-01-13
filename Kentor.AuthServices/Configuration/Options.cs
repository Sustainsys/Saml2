using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Configuration;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Options implementation for handling in memory options.
    /// </summary>
    public class Options : IOptions
    {
        /// <summary>
        /// Reads the options from the current config file.
        /// </summary>
        /// <returns>Options object.</returns>
        public static Options FromConfiguration
        {
            get
            {
                var options = new Options(KentorAuthServicesSection.Current);
                KentorAuthServicesSection.Current.IdentityProviders.RegisterIdentityProviders(options);
                KentorAuthServicesSection.Current.Federations.RegisterFederations(options);

                // Load the federation configuration section from config.
                if (!Options.suppressFederationIdentityLoading)
                {
                    FederationIdentityConfiguration federationIdentity = new FederationIdentityConfiguration();
                    federationIdentity.FromConfiguration();
                    options.FederationIdentityConfiguration(federationIdentity);
                }
                return options;
            }
        }

        private static bool suppressFederationIdentityLoading;

        /// <summary>
        /// Used for tests to avoid exceptions due to unrecognized types in the identityConfiguration section. 
        /// </summary>
        public static bool SuppressFederationIdentityLoading
        {
            get
            {
                return suppressFederationIdentityLoading;
            }
            set
            {
                suppressFederationIdentityLoading = value;
            }
        }


        /// <summary>
        /// Creates an options object with the specified SPOptions.
        /// </summary>
        /// <param name="spOptions"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public Options(ISPOptions spOptions)
        {
            this.spOptions = spOptions;
        }

        private readonly ISPOptions spOptions;

        /// <summary>
        /// Options for the service provider's behaviour; i.e. everything except
        /// the idp and federation list.
        /// </summary>
        public ISPOptions SPOptions
        {
            get
            {
                return spOptions;
            }
        }

        /// <summary>
        /// Sets the <c>FederationIdentityConfiguration</c> of the configuration options. 
        /// </summary>
        /// <param name="identity">The <c>FederationIdentityConfiguration</c> to be set.</param>
        private void FederationIdentityConfiguration(FederationIdentityConfiguration identity)
        {
            federationIdentityConfiguration = identity;
        }

        /// <summary>
        /// Property to get the identity configuration section of the read federation configuration section.
        /// </summary>
        public IdentityConfiguration IdentityConfiguration
        {
            get
            {
                return federationIdentityConfiguration.IdentityConfiguration;
            }
        }

        private FederationIdentityConfiguration federationIdentityConfiguration;


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
