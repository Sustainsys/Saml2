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
    /// Shared instance, the class has no state, it is only a normal
    /// class to enable inheritance.
    /// </summary>
    public static MetadataSerializer Instance {get;} = new();

    /// <summary>
    /// Helper method that calls ThrowOnErrors. If you want to supress
    /// errors and prevent throwing, this is the last chance method to
    /// override.
    /// </summary>
    /// <param name="xmlTraverser">XmlTraverser to call ThrowOnErrors on.</param>
    protected virtual void ThrowOnErrors(XmlTraverser xmlTraverser)
        => xmlTraverser.ThrowOnErrors();

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
        xmlTraverser.EnsureName(Namespaces.Metadata, "EntityDescriptor");
        
        var entityDescriptor = CreateEntityDescriptor();
        
        ReadAttributes(xmlTraverser, entityDescriptor);
        
        using (xmlTraverser.EnterChildLevel())
        {
            ReadElements(xmlTraverser, entityDescriptor);
        }

        ThrowOnErrors(xmlTraverser);

        return entityDescriptor;
    }

    /// <summary>
    /// Read attributes of EntityDescriptor
    /// </summary>
    /// <param name="xmlTraverser">Xml data to read</param>
    /// <param name="entityDescriptor">EntityDescriptor</param>
    protected virtual void ReadAttributes(XmlTraverser xmlTraverser, EntityDescriptor entityDescriptor)
    {
        entityDescriptor.EntityId = xmlTraverser.GetRequiredAbsoluteUriAttribute("entityID");
        entityDescriptor.Id = xmlTraverser.GetAttribute("ID");
        entityDescriptor.CacheDuraton = xmlTraverser.GetTimeSpanAttribute("cacheDuration");
        entityDescriptor.ValidUntil = xmlTraverser.GetDateTimeAttribute("validUntil");
    }

    /// <summary>
    /// Read the child elements of the EntityDescriptor.
    /// </summary>
    /// <param name="xmlTraverser">Xml data to read</param>
    /// <param name="entityDescriptor">Entity Descriptor to populate</param>
    protected virtual void ReadElements(XmlTraverser xmlTraverser, EntityDescriptor entityDescriptor)
    {
        if (!xmlTraverser.MoveToNextRequiredChild())
            return; // Abort, errors will be reported.
    }
}
