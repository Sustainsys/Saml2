using Sustainsys.Saml2.Metadata.Attributes;
using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.Metadata;

/// <summary>
/// Serializer for Saml2 Metadata
/// </summary>
public partial class MetadataSerializer : SerializerBase
{
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
            ReadElements(source.GetChildren(), entityDescriptor);
        }

        source.MoveNext(true);

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
        if (!source.MoveNext())
        {
            return;
        }

        if (source.ReadAndValidateOptionalSignature(
            TrustedSigningKeys, AllowedHashAlgorithms, out var trustLevel))
        {
            entityDescriptor.TrustLevel = trustLevel;

            if (!source.MoveNext())
            {
                return;
            }
        }

        if (source.HasName(Namespaces.Metadata, ElementNames.Extensions))
        {
            entityDescriptor.Extensions = ReadExtensions(source);
            if (!source.MoveNext())
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
                        source.IgnoreChildren();
                        break;
                    default:
                        wasRoleDescriptor = false; // Nope, something else.
                        break;
                }
            }
        } while (wasRoleDescriptor && source.MoveNext(true));
    }
}