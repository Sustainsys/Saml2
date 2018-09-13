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
