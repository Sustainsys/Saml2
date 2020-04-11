using Sustainsys.Saml2.Metadata.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sustainsys.Saml2.Metadata.Descriptors
{
    public abstract class WebServiceDescriptor : RoleDescriptor
    {
        public bool? AutomaticPseudonyms { get; set; }

        public ICollection<Uri> ClaimDialectsOffered { get; private set; } =
            new Collection<Uri>();

        public ICollection<DisplayClaim> ClaimTypesOffered { get; private set; } =
            new Collection<DisplayClaim>();

        public ICollection<DisplayClaim> ClaimTypesRequested { get; private set; } =
            new Collection<DisplayClaim>();

        public ICollection<Uri> LogicalServiceNamesOffered { get; private set; } =
            new Collection<Uri>();

        public string ServiceDescription { get; set; }
        public string ServiceDisplayName { get; set; }

        public ICollection<EndpointReference> TargetScopes { get; private set; } =
            new Collection<EndpointReference>();

        public ICollection<Uri> TokenTypesOffered { get; private set; } =
            new Collection<Uri>();

        protected WebServiceDescriptor()
        {
        }
    }
}