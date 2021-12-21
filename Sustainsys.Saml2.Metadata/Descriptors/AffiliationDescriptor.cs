using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Sustainsys.Saml2.Metadata.Descriptors
{
    public class AffiliationDescriptor : ICachedMetadata
    {
        public ICollection<EntityId> AffiliateMembers { get; private set; } =
            new Collection<EntityId>();

        public ICollection<XmlElement> Extensions { get; private set; } =
            new Collection<XmlElement>();

        public ICollection<KeyDescriptor> KeyDescriptors { get; private set; } =
            new Collection<KeyDescriptor>();

        public EntityId AffiliationOwnerId { get; set; }
        public DateTime? ValidUntil { get; set; }
        public XsdDuration? CacheDuration { get; set; }
        public string Id { get; set; }
    }
}