using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Sustainsys.Saml2.Metadata
{
	public class DiscoveryResponse : IndexedEndpoint
	{
	}

	public class SpSsoDescriptor : SsoDescriptor
	{
		public IndexedCollectionWithDefault<AssertionConsumerService> AssertionConsumerServices { get; private set; } =
			new IndexedCollectionWithDefault<AssertionConsumerService>();
		public IndexedCollectionWithDefault<AttributeConsumingService> AttributeConsumingServices { get; private set; } =
			new IndexedCollectionWithDefault<AttributeConsumingService>();
		public bool? AuthnRequestsSigned { get; set; }
		public bool? WantAssertionsSigned { get; set; }
		public IndexedCollectionWithDefault<DiscoveryResponse> DiscoveryResponses { get; private set; } =
			new IndexedCollectionWithDefault<DiscoveryResponse>();
	}
}
