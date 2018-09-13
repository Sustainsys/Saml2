using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Sustainsys.Saml2.Metadata
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
