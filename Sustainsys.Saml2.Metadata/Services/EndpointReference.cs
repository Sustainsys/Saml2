using System;
using System.Collections.ObjectModel;
using System.Xml;

namespace Sustainsys.Saml2.Metadata.Services
{
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

        // TODO : return 'set' to internal once all codes related to metadata has been moved
        public Uri Uri { get; set; }

        // TODO : return this to internal once all codes related to metadata has been moved
        public EndpointReference()
        {
        }

        public EndpointReference(string uri)
        {
            Uri = new Uri(uri);
        }
    }
}