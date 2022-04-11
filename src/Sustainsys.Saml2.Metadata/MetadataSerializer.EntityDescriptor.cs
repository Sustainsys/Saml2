using Sustainsys.Saml2.Metadata.Attributes;
using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Xml;

namespace Sustainsys.Saml2.Metadata;

/// <summary>
/// Serializer for Saml2 Metadata
/// </summary>
public partial class MetadataSerializer
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

        if (source.EnsureName(Namespaces.Metadata, ElementNames.EntityDescriptor))
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
        entityDescriptor.EntityId = source.GetRequiredAbsoluteUriAttribute(AttributeNames.entityID) ?? "";
        entityDescriptor.Id = source.GetAttribute(AttributeNames.ID);
        entityDescriptor.CacheDuraton = source.GetTimeSpanAttribute(AttributeNames.cacheDuration);
        entityDescriptor.ValidUntil = source.GetDateTimeAttribute(AttributeNames.validUntil);
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

            if (!source.MoveToNextRequiredChild())
            {
                return;
            }
        }

        if (source.HasName(Namespaces.Metadata, ElementNames.Extensions))
        {
            entityDescriptor.Extensions = ReadExtensions(source);
            if (!source.MoveToNextRequiredChild())
            {
                return;
            }
        }

        // Now we're at the actual role descriptors - or possibly an AffiliationDescriptor.
        bool wasRoleDescriptor = true; // Assume the best.
        do
        {
            if(source.EnsureNamespace(Namespaces.Metadata))
            {
                switch (source.CurrentNode.LocalName)
                {
                    case ElementNames.RoleDescriptor:
                        entityDescriptor.RoleDescriptors.Add(ReadRoleDescriptor(source));
                        break;
                    case ElementNames.IDPSSODescriptor:
                        entityDescriptor.RoleDescriptors.Add(ReadIDPSSODescriptor(source));
                        break;
                    case ElementNames.SPSSODescriptor:
                    case ElementNames.AuthnAuthorityDescriptor:
                    case ElementNames.AttributeAuthorityDescriptor:
                    case ElementNames.PDPDescriptor:
                        break;
                    default:
                        wasRoleDescriptor = false; // Nope, something else.
                        break;
                }
            }
        } while (wasRoleDescriptor && source.MoveToNextChild());

        // We're not reading the organization information for now, just skip the rest.
        source.SkipChildren();
    }
}