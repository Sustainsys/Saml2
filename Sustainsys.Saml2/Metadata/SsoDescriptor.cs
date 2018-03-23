using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sustainsys.Saml2.Metadata
{
	public class SsoDescriptor : RoleDescriptor
    {
		public ICollection<ArtifactResolutionService> ArtifactResolutionServices { get; private set; } =
			new Collection<ArtifactResolutionService>();
		public ICollection<SingleLogoutService> SingleLogoutServices { get; private set; } =
			new Collection<SingleLogoutService>();
		public ICollection<ManageNameIDService> ManageNameIDServices { get; private set; } =
			new Collection<ManageNameIDService>();
		public ICollection<NameIDFormat> NameIdentifierFormats { get; private set; } =
			new Collection<NameIDFormat>();
	}
}
