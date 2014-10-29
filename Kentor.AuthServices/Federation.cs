using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Metadata;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Represents a federation known to this service provider.
    /// </summary>
    public class Federation
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="config">Config to use to initialize the federation.</param>
        /// <param name="options">Options to pass on to created IdentityProvider
        /// instances and register identity providers in.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public Federation(FederationElement config, IOptions options)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            Init(config.MetadataUrl, config.AllowUnsolicitedAuthnResponse, options);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="metadataUrl">Url to where metadata can be fetched.</param>
        /// <param name="allowUnsolicitedAuthnResponse">Should unsolicited responses 
        /// from idps in this federation be accepted?</param>
        /// <param name="options">Options to pass on to created IdentityProvider
        /// instances and register identity providers in.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "sp")]
        public Federation(Uri metadataUrl, bool allowUnsolicitedAuthnResponse, Options options)
        {
            Init(metadataUrl, allowUnsolicitedAuthnResponse, options);
        }

        private bool allowUnsolicitedAuthnResponse;
        private IOptions options;
        private Uri metadataUrl;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "metadataUrl"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "allowUnsolicitedAuthnResponse"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "options")]
        private void Init(Uri metadataUrl, bool allowUnsolicitedAuthnResponse, IOptions options)
        {
            this.allowUnsolicitedAuthnResponse = allowUnsolicitedAuthnResponse;
            this.options = options;
            this.metadataUrl = metadataUrl;

            LoadMetadata();
        }

        private void LoadMetadata()
        {
            try
            {
                var metadata = MetadataLoader.LoadFederation(metadataUrl);

                var identityProviders = metadata.ChildEntities.Cast<ExtendedEntityDescriptor>()
                    .Where(ed => ed.RoleDescriptors.OfType<IdentityProviderSingleSignOnDescriptor>().Any())
                    .Select(ed => new IdentityProvider(ed, allowUnsolicitedAuthnResponse, options.SPOptions))
                    .ToList();

                RegisterIdentityProviders(identityProviders);

                MetadataValidUntil = metadata.ValidUntil;

                if (metadata.CacheDuration.HasValue)
                {
                    MetadataValidUntil = DateTime.UtcNow.Add(metadata.CacheDuration.Value);
                }
            }
            catch (WebException)
            {
                // If download failed, ignore the error and trigger a scheduled reload.
                MetadataValidUntil = DateTime.MinValue;
            }
        }

        // Use a string and not EntityId as List<> doesn't support setting a
        // custom equality comparer as required to handle EntityId correctly.
        private IDictionary<string, EntityId> registeredIdentityProviders = new Dictionary<string, EntityId>();

        private void RegisterIdentityProviders(List<IdentityProvider> identityProviders)
        {
            // Add or update the idps in the new metadata.
            foreach (var idp in identityProviders)
            {
                options.IdentityProviders[idp.EntityId] = idp;
                registeredIdentityProviders.Remove(idp.EntityId.Id);
            }

            // Remove idps from previous set of metadata that were not updated now.
            foreach(var idp in registeredIdentityProviders.Values)
            {
                options.IdentityProviders.Remove(idp);
            }

            // Remember what we registered this time, to know what to remove nex time.
            foreach(var idp in identityProviders)
            {
                registeredIdentityProviders = identityProviders.ToDictionary(
                    i => i.EntityId.Id,
                    i => i.EntityId);
            }
        }

        private DateTime? metadataValidUntil;

        /// <summary>
        /// For how long is the metadata that the federation has loaded valid?
        /// Null if there is no limit.
        /// </summary>
        public DateTime? MetadataValidUntil
        {
            get
            {
                return metadataValidUntil;
            }
            private set
            {
                metadataValidUntil = value;

                if(value.HasValue)
                {
                    Task.Delay(MetadataRefreshScheduler.GetDelay(value.Value))
                        .ContinueWith((_) => LoadMetadata());
                }
            }
        }
    }
}
