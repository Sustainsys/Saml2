using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
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

		#if FALSE
		public static EndpointReference ReadFrom(XmlDictionaryReader reader)
		{
			throw new NotImplementedException();
		}

		public static EndpointReference ReadFrom(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		public static void WriteTo(XmlWriter writer)
		{
			throw new NotImplementedException();
		}
		#endif
	}
}
