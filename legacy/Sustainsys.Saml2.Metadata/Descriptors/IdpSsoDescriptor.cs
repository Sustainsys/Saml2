using Microsoft.IdentityModel.Tokens.Saml2;
using Sustainsys.Saml2.Metadata.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sustainsys.Saml2.Metadata.Descriptors
{
    public class IdpSsoDescriptor : SsoDescriptor
    {
        public ICollection<SingleSignOnService> SingleSignOnServices { get; private set; } =
            new Collection<SingleSignOnService>();

        public ICollection<NameIDMappingService> NameIDMappingServices { get; private set; } =
            new Collection<NameIDMappingService>();

        public ICollection<AssertionIdRequestService> AssertionIDRequestServices { get; private set; } =
            new Collection<AssertionIdRequestService>();

        public ICollection<AttributeProfile> AttributeProfiles { get; private set; } =
            new Collection<AttributeProfile>();

        public ICollection<Saml2Attribute> SupportedAttributes { get; private set; } =
            new Collection<Saml2Attribute>();

        public bool? WantAuthnRequestsSigned { get; set; }
    }
}