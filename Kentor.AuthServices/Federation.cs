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
        readonly IDictionary<EntityId, IdentityProvider> identityProviders;

        public IDictionary<EntityId, IdentityProvider> IdentityProviders
        {
            get
            {
                return identityProviders;
            }
        }

        public Federation(FederationElement config)
        {
            var metadata = MetadataLoader.LoadFederation(config.MetadataUrl);

            identityProviders = metadata.ChildEntities
                .Where(ed => ed.RoleDescriptors.OfType<IdentityProviderSingleSignOnDescriptor>().Any())
                .Select(ed => new IdentityProvider(ed, config.AllowUnsolicitedAuthnResponse))
                .ToDictionary(idp => idp.EntityId, EntityIdEqualityComparer.Instance);
        }
    }
}
 