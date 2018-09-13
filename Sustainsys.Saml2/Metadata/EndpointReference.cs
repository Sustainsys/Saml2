using System;
using System.Collections.ObjectModel;
using System.Xml;

namespace Sustainsys.Saml2.Metadata
{
	public class ServiceName
	{
		public string PortName { get; set; }
		public string Name { get; set; }
	}

	public class EndpointReference
	{
		public Collection<XmlElement> Metadata { get; private set; } =
			new Collection<XmlElement>();
		public Collection<XmlElement> ReferenceProperties { get; private set; } =
			new Collection<XmlElement>();
		public Collection<XmlElement> ReferenceParameters { get; private set; } =
			new Collection<XmlElement>();
		public Collection<XmlElement> Policies { get; private set; } =
			new Collection<XmlElement>();
		public string PortType { get; set; }
		public ServiceName ServiceName { get; set; }
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
