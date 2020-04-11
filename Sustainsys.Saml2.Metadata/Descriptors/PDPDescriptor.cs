using Sustainsys.Saml2.Metadata.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sustainsys.Saml2.Metadata.Descriptors
{
    public class PDPDescriptor : RoleDescriptor
    {
        public ICollection<AuthzService> AuthzServices { get; private set; } =
            new Collection<AuthzService>();

        public ICollection<AssertionIdRequestService> AssertionIdRequestServices { get; private set; } =
            new Collection<AssertionIdRequestService>();

        public ICollection<NameIDFormat> NameIDFormats { get; private set; } =
            new Collection<NameIDFormat>();
    }
}