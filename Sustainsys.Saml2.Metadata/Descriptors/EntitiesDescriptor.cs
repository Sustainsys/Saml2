using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Sustainsys.Saml2.Metadata.Descriptors
{
    public class EntitiesDescriptor : MetadataBase, ICachedMetadata
    {
        public ICollection<EntityDescriptor> ChildEntities { get; private set; } =
            new Collection<EntityDescriptor>();

        public ICollection<EntitiesDescriptor> ChildEntityGroups { get; private set; } =
            new Collection<EntitiesDescriptor>();

        public Collection<XmlElement> Extensions { get; private set; } =
            new Collection<XmlElement>();

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? ValidUntil { get; set; }
        public XsdDuration? CacheDuration { get; set; }
    }
}