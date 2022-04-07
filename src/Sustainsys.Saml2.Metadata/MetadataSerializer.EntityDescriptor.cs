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
    /// Allowed hash algorithms if validating signatures.
    /// </summary>
    protected IEnumerable<string>? AllowedHashAlgorithms { get; }

    /// <summary>
    /// Signing keys to trust when validating signatures of the metadata.
    /// </summary>
    protected IEnumerable<SigningKey>? TrustedSigningKeys { get; }

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="trustedSigningKeys">Trusted signing keys to use to validate any signature found.</param>
    /// <param name="allowedHashAlgorithms">Allowed hash algorithms if validating signatures.</param>
    public MetadataSerializer(
        IEnumerable<SigningKey>? trustedSigningKeys,
        IEnumerable<string>? allowedHashAlgorithms)
    {
        TrustedSigningKeys = trustedSigningKeys;
        AllowedHashAlgorithms = allowedHashAlgorithms;
    }

    /// <summary>
    /// Helper method that calls ThrowOnErrors. If you want to supress
    /// errors and prevent throwing, this is the last chance method to
    /// override.
    /// </summary>
    protected virtual void ThrowOnErrors(XmlTraverser source)
        => source.ThrowOnErrors();

    /// <summary>
    /// Create EntityDescriptor instance. Override to use subclass.
    /// </summary>
    /// <returns>EntityDescriptor</returns>
    protected virtual EntityDescriptor CreateEntityDescriptor() => new();

    /// <summary>
    /// Read an EntityDescriptor
    /// </summary>
    /// <returns>EntityDescriptor</returns>
    public virtual EntityDescriptor ReadEntityDescriptor(XmlTraverser source)
    {
        var entityDescriptor = CreateEntityDescriptor();
        
        if(source.EnsureName(Namespaces.Metadata, "EntityDescriptor"))
        {
            ReadAttributes(source, entityDescriptor);

            using (source.EnterChildLevel())
            {
                ReadElements(source, entityDescriptor);
            }
        }

        ThrowOnErrors(source);

        return entityDescriptor;
    }

    /// <summary>
    /// Read attributes of EntityDescriptor
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="entityDescriptor">EntityDescriptor</param>
    protected virtual void ReadAttributes(XmlTraverser source, EntityDescriptor entityDescriptor)
    {
        entityDescriptor.EntityId = source.GetRequiredAbsoluteUriAttribute("entityID");
        entityDescriptor.Id = source.GetAttribute("ID");
        entityDescriptor.CacheDuraton = source.GetTimeSpanAttribute("cacheDuration");
        entityDescriptor.ValidUntil = source.GetDateTimeAttribute("validUntil");
    }

    /// <summary>
    /// Read the child elements of the EntityDescriptor.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="entityDescriptor">Entity Descriptor to populate</param>
    protected virtual void ReadElements(XmlTraverser source, EntityDescriptor entityDescriptor)
    {
        if (!source.MoveToNextRequiredChild())
        {
            return;
        }

        if (source.ReadAndValidateOptionalSignature(
            TrustedSigningKeys, AllowedHashAlgorithms, out var trustLevel))
        {
            entityDescriptor.TrustLevel = trustLevel;

            if(!source.MoveToNextRequiredChild())
            {
                return;
            }
        }

        if(ReadExtensions(source, entityDescriptor)
            && !source.MoveToNextRequiredChild())
        {
            return;
        }


    }

    /// <summary>
    /// Process extensions node. Default just checks if this is an extensions node and then returns.
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="entityDescriptor">Currently processed EntityDescriptor</param>
    /// <returns>True if current node was an Extensions element</returns>
    protected virtual bool ReadExtensions(XmlTraverser source, EntityDescriptor entityDescriptor)
    {
        return source.CurrentNode.LocalName == "Extensions"
            && source.CurrentNode.NamespaceURI == Namespaces.Metadata;
    }
}