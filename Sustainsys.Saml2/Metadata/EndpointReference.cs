using System;
using System.Collections.ObjectModel;
using System.Xml;

namespace Sustainsys.Saml2.Metadata
{
	public class EndpointReference
	{
		public Collection<XmlElement> ReferenceParameters { get; private set; } =
			new Collection<XmlElement>();
		public Collection<XmlElement> Metadata { get; private set; } =
			new Collection<XmlElement>();
		public Uri Uri { get; internal set; }

		internal EndpointReference()
		{
		}

		public EndpointReference(string uri)
		{
			Uri = new Uri(uri);
		}
	}
}
