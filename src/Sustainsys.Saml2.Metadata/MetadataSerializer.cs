using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Metadata;

public class MetadataSerializer
{
    public EntityDescriptor ReadEntityDescriptor(XmlTraverser xmlTraverser)
    {
        xmlTraverser.EnsureName(Namespaces.Metadata, "EntityDescriptor");

        return new EntityDescriptor()
        {
            EntityId = xmlTraverser.GetRequiredAttribute("entityID"),
            Id = xmlTraverser.GetAttribute("ID"),
            CacheDuraton = xmlTraverser.GetTimeSpanAttribute("cacheDuration"),
            ValidUntil = xmlTraverser.GetDateTimeAttribute("validUntil")
        };
    }
}
