using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Metadata;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Represents a federation known to this service provider.
    /// </summary>
    public class Federation
    {
        List<IdentityProvider> identityProviders;
        IList<IdentityProvider> readonlyIdentityProviders;

        /// <summary>
        /// The identity providers in the federation.
        /// </summary>
        public IEnumerable<IdentityProvider> IdentityProviders
        {
            get
            {
                return readonlyIdentityProviders;
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="config">Config to use to initialize the federation.</param>
        /// <param name="spOptions">Service provider options to pass on to
        /// created IdentityProvider instances.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public Federation(FederationElement config, ISPOptions spOptions)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            Init(config.MetadataUrl, config.AllowUnsolicitedAuthnResponse, spOptions);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="metadataUrl">Url to where metadata can be fetched.</param>
        /// <param name="allowUnsolicitedAuthnResponse">Should unsolicited responses 
        /// from idps in this federation be accepted?</param>
        /// <param name="spOptions">Service provider options to pass on to
        /// created IdentityProvider instances.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public Federation(Uri metadataUrl, bool allowUnsolicitedAuthnResponse, ISPOptions spOptions)
        {
            Init(metadataUrl, allowUnsolicitedAuthnResponse, spOptions);
        }

        private void Init(Uri metadataUrl, bool allowUnsolicitedAuthnResponse, ISPOptions spOptions)
        {
            Init(MetadataLoader.LoadFederation(metadataUrl), allowUnsolicitedAuthnResponse, spOptions);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="metadata">Metadata to initialize this federation from.</param>
        /// <param name="allowUnsolicitedAuthnResponse">Should unsolicited responses 
        /// from idps in this federation be accepted?</param>
        /// <param name="spOptions">Service provider options to pass on to
        /// created IdentityProvider instances.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public Federation(EntitiesDescriptor metadata, bool allowUnsolicitedAuthnResponse, ISPOptions spOptions)
        {
            Init(metadata, allowUnsolicitedAuthnResponse, spOptions);
        }

        private void Init(EntitiesDescriptor metadata, bool allowUnsolicitedAuthnResponse, ISPOptions spOptions)
        {
            identityProviders = metadata.ChildEntities.Cast<ExtendedEntityDescriptor>()
                .Where(ed => ed.RoleDescriptors.OfType<IdentityProviderSingleSignOnDescriptor>().Any())
                .Select(ed => new IdentityProvider(ed, allowUnsolicitedAuthnResponse, spOptions))
                .ToList();

            readonlyIdentityProviders = identityProviders.AsReadOnly();
        }
    }
}
