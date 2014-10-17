using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices
{
    class Federation
    {
        readonly List<IdentityProvider> identityProviders;
        readonly IList<IdentityProvider> readonlyIdentityProviders;

        [Obsolete]
        public IDictionary<EntityId, IdentityProvider> IdentityProviderDictionary
        {
            get
            {
                return identityProviders.ToDictionary(i => i.EntityId, EntityIdEqualityComparer.Instance);
            }
        }

        public IEnumerable<IdentityProvider> IdentityProviders
        {
            get
            {
                return readonlyIdentityProviders;
            }
        }

        public Federation(FederationElement config)
            : this(config.MetadataUrl, config.AllowUnsolicitedAuthnResponse)
        {
        }

        public Federation(Uri metadataUrl, bool allowUnsolicitedAuthnResponse)
            : this(MetadataLoader.LoadFederation(metadataUrl), allowUnsolicitedAuthnResponse)
        {
        }

        internal Federation(EntitiesDescriptor metadata, bool allowUnsolicitedAuthnResponse)
        {
            identityProviders = metadata.ChildEntities
                .Where(ed => ed.RoleDescriptors.OfType<IdentityProviderSingleSignOnDescriptor>().Any())
                .Select(ed => new IdentityProvider(ed, allowUnsolicitedAuthnResponse))
                .ToList();

            readonlyIdentityProviders = identityProviders.AsReadOnly();
        }
    }
}
 