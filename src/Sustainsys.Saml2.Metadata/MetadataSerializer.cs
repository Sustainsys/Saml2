using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Metadata;

/// <summary>
/// Serializer for Saml2 Metadata
/// </summary>
public class MetadataSerializer
{
    /// <summary>
    /// Read an EntityDescriptor
    /// </summary>
    /// <param name="xmlTraverser">Source data</param>
    /// <returns>EntityDescriptor</returns>
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
