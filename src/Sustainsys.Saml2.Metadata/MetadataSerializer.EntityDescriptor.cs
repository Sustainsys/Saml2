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
    /// Create EntityDescriptor instance. Override to use subclass.
    /// </summary>
    /// <returns>EntityDescriptor</returns>
    protected virtual EntityDescriptor CreateEntityDescriptor() => new();

    /// <summary>
    /// Read an EntityDescriptor
    /// </summary>
    /// <param name="xmlTraverser">Source data</param>
    /// <returns>EntityDescriptor</returns>
    public virtual EntityDescriptor ReadEntityDescriptor(XmlTraverser xmlTraverser)
    {
        var entityDescriptor = CreateEntityDescriptor();

        ReadAttributes(xmlTraverser, entityDescriptor);
       
        Validate(xmlTraverser, entityDescriptor);

        xmlTraverser.ThrowOnErrors();

        return entityDescriptor;
    }

    /// <summary>
    /// Validates the EntityDescriptor
    /// </summary>
    /// <param name="xmlTraverser">Xml Traverser source</param>
    /// <param name="entityDescriptor">Parsed entity descriptor</param>
    protected virtual void Validate(XmlTraverser xmlTraverser, EntityDescriptor entityDescriptor)
    {
        xmlTraverser.EnsureName(Namespaces.Metadata, "EntityDescriptor");
    }

    /// <summary>
    /// Read attributes of EntityDescriptor
    /// </summary>
    /// <param name="xmlTraverser">Xml data to read</param>
    /// <param name="entityDescriptor">EntityDescriptor</param>
    protected virtual void ReadAttributes(XmlTraverser xmlTraverser, EntityDescriptor entityDescriptor)
    {
        entityDescriptor.EntityId = xmlTraverser.GetRequiredAttribute("entityID");
        entityDescriptor.Id = xmlTraverser.GetAttribute("ID");
        entityDescriptor.CacheDuraton = xmlTraverser.GetTimeSpanAttribute("cacheDuration");
        entityDescriptor.ValidUntil = xmlTraverser.GetDateTimeAttribute("validUntil");
    }
}
