using Kentor.AuthServices.Configuration;
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
        public Federation(FederationElement config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            Init(config.MetadataUrl, config.AllowUnsolicitedAuthnResponse);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="metadataUrl">Url to where metadata can be fetched.</param>
        /// <param name="allowUnsolicitedAuthnResponse">Should unsolicited responses 
        /// from idps in this federation be accepted?</param>
        public Federation(Uri metadataUrl, bool allowUnsolicitedAuthnResponse)
        {
            Init(metadataUrl, allowUnsolicitedAuthnResponse);
        }

        private void Init(Uri metadataUrl, bool allowUnsolicitedAuthnResponse)
        {
            Init(MetadataLoader.LoadFederation(metadataUrl), allowUnsolicitedAuthnResponse);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="metadata">Metadata to initialize this federation from.</param>
        /// <param name="allowUnsolicitedAuthnResponse">Should unsolicited responses 
        /// from idps in this federation be accepted?</param>
        public Federation(EntitiesDescriptor metadata, bool allowUnsolicitedAuthnResponse)
        {
            Init(metadata, allowUnsolicitedAuthnResponse);
        }

        private void Init(EntitiesDescriptor metadata, bool allowUnsolicitedAuthnResponse)
        {
            identityProviders = metadata.ChildEntities
                .Where(ed => ed.RoleDescriptors.OfType<IdentityProviderSingleSignOnDescriptor>().Any())
                .Select(ed => new IdentityProvider(ed, allowUnsolicitedAuthnResponse))
                .ToList();

            readonlyIdentityProviders = identityProviders.AsReadOnly();
        }
    }
}
