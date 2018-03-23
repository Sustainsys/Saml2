using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Security.Claims;

namespace Sustainsys.Saml2.Metadata
{
	public class ApplicationServiceDescriptor : WebServiceDescriptor
	{
		public ICollection<EndpointReference> Endpoints { get; private set; } =
			new Collection<EndpointReference>();
		public ICollection<EndpointReference> PassiveRequestorEndpoints { get; private set; } =
			new Collection<EndpointReference>();
		public ICollection<EndpointReference> SingleSignOutEndpoints { get; private set; } =
			new Collection<EndpointReference>();
	}
}
