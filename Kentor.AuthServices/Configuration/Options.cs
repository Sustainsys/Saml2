using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private static Lazy<Options> configuredOptions =
            new Lazy<Options>(ReadFromConfiguration, true);

        /// <summary>
        /// Reads the options from the current config file.
        /// </summary>
        /// <returns>Options object.</returns>
        public static Options FromConfiguration
        {
            get
            {
                return configuredOptions.Value;
            }
        }

        private static Options ReadFromConfiguration()
        {
            return new Options(KentorAuthServicesSection.Current);
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

        private readonly ConcurrentDictionary<EntityId, IdentityProvider> identityProviders
            = new ConcurrentDictionary<EntityId, IdentityProvider>();

        /// <summary>
        /// Available identity providers.
        /// </summary>
        public ConcurrentDictionary<EntityId, IdentityProvider> IdentityProviders
        {
            get
            {
                return identityProviders;
            }
        }
    }
}
