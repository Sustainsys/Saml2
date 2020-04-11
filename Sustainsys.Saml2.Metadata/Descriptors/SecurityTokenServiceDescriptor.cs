using Sustainsys.Saml2.Metadata.Services;
using System.Collections.ObjectModel;

namespace Sustainsys.Saml2.Metadata.Descriptors
{
    public class SecurityTokenServiceDescriptor : WebServiceDescriptor
    {
        public Collection<EndpointReference> SecurityTokenServiceEndpoints { get; private set; } =
            new Collection<EndpointReference>();

        public Collection<EndpointReference> SingleSignOutSubscriptionEndpoints { get; private set; } =
            new Collection<EndpointReference>();

        public Collection<EndpointReference> SingleSignOutNotificationEndpoints { get; private set; } =
            new Collection<EndpointReference>();

        public Collection<EndpointReference> PassiveRequestorEndpoints { get; private set; } =
            new Collection<EndpointReference>();
    }
}