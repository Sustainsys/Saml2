using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Kentor.AuthServices.Configuration
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
        public KentorAuthServicesNotifications Notifications { get; set; }

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
            var spOptions = new SPOptions(KentorAuthServicesSection.Current);
            var options = new Options(spOptions);
            KentorAuthServicesSection.Current.IdentityProviders.RegisterIdentityProviders(options);
            KentorAuthServicesSection.Current.Federations.RegisterFederations(options);

            return options;
        }

        /// <summary>
        /// Creates an options object with the specified SPOptions.
        /// </summary>
        /// <param name="spOptions"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public Options(SPOptions spOptions)
        {
            Notifications = new KentorAuthServicesNotifications();
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

        internal const string RsaSha256Uri = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        internal const string Sha256Uri = "http://www.w3.org/2001/04/xmlenc#sha256";

        /// <summary>
        /// Make Sha256 signature algorithm available in this process (not just Kentor.AuthServices)
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sha" )]
        public static void GlobalEnableSha256XmlSignatures()
        {
            CryptoConfig.AddAlgorithm(typeof(ManagedSHA256SignatureDescription), RsaSha256Uri);

            AddAlgorithmIfMissing((IList<string>)XmlHelpers.KnownSigningAlgorithms, RsaSha256Uri);
            AddAlgorithmIfMissing((IList<string>)XmlHelpers.DigestAlgorithms, Sha256Uri);
        }

        internal static void AddAlgorithmIfMissing(IList<string> knownAlgorithms, string newAlgorithm)
        {
            if (knownAlgorithms.Count == 1)
            {
                knownAlgorithms.Add(newAlgorithm);
            }
        }
    }
}
