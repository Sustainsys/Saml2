using Sustainsys.Saml2.Metadata.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sustainsys.Saml2.Metadata.Descriptors
{
    public class SsoDescriptor : RoleDescriptor
    {
        public IndexedCollectionWithDefault<ArtifactResolutionService> ArtifactResolutionServices
        { get; private set; } = new IndexedCollectionWithDefault<ArtifactResolutionService>();

        public ICollection<SingleLogoutService> SingleLogoutServices { get; private set; } =
            new Collection<SingleLogoutService>();

        public ICollection<ManageNameIDService> ManageNameIDServices { get; private set; } =
            new Collection<ManageNameIDService>();

        public ICollection<NameIDFormat> NameIdentifierFormats { get; private set; } =
            new Collection<NameIDFormat>();
    }
}