using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Sustainsys.Saml2.Metadata.Descriptors
{
    public abstract class RoleDescriptor : ICachedMetadata
    {
        public ICollection<ContactPerson> Contacts { get; private set; }
            = new Collection<ContactPerson>();

        public ICollection<Uri> ProtocolsSupported { get; private set; }
        public Uri ErrorUrl { get; set; }
        public string Id { get; set; }

        public ICollection<KeyDescriptor> Keys { get; private set; }
            = new Collection<KeyDescriptor>();

        public Organization Organization { get; set; }
        public XsdDuration? CacheDuration { get; set; }
        public DateTime? ValidUntil { get; set; }

        public ICollection<XmlElement> Extensions { get; private set; } =
            new Collection<XmlElement>();

        protected RoleDescriptor() :
            this(new Collection<Uri>())
        {
        }

        protected RoleDescriptor(Collection<Uri> protocolsSupported)
        {
            ProtocolsSupported = protocolsSupported;
        }
    }
}