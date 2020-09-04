using Sustainsys.Saml2.Metadata.Services;
using System.Collections.Generic;

namespace Sustainsys.Saml2.Metadata.Descriptors
{
    public class ApplicationServiceDescriptor : WebServiceDescriptor
    {
        public ICollection<EndpointReference> Endpoints { get; private set; } =
            new List<EndpointReference>();

        public ICollection<EndpointReference> PassiveRequestorEndpoints { get; private set; } =
            new List<EndpointReference>();

        public ICollection<EndpointReference> SingleSignOutEndpoints { get; private set; } =
            new List<EndpointReference>();

        public ApplicationServiceDescriptor()
        {
        }

        public ApplicationServiceDescriptor(
            IEnumerable<EndpointReference> endpoints,
            IEnumerable<EndpointReference> passiveRequestorEndpoints,
            IEnumerable<EndpointReference> singleSignOutEndpoints
        )
        {
            ((List<EndpointReference>)Endpoints).AddRange(endpoints);
        }
    }
}