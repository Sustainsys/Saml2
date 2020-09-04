using Microsoft.IdentityModel.Tokens.Saml2;
using Sustainsys.Saml2.Metadata.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sustainsys.Saml2.Metadata.Descriptors
{
#if FALSE
	public class SamlAttribute
	{
		public string Name { get; set; }
		public Uri NameFormat { get; set; }
		public string FriendlyName { get; set; }
		public string Value { get; set; }
	}
#endif

    public class AttributeAuthorityDescriptor : RoleDescriptor
    {
        public ICollection<AttributeService> AttributeServices { get; private set; } =
            new Collection<AttributeService>();

        public ICollection<AssertionIdRequestService> AssertionIdRequestServices { get; private set; } =
            new Collection<AssertionIdRequestService>();

        public ICollection<NameIDFormat> NameIDFormats { get; private set; } =
            new Collection<NameIDFormat>();

        public ICollection<AttributeProfile> AttributeProfiles { get; private set; } =
            new Collection<AttributeProfile>();

        public ICollection<Saml2Attribute> Attributes { get; private set; } =
            new Collection<Saml2Attribute>();
    }
}