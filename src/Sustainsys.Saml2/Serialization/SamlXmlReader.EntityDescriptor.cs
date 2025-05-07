using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlReader
{
    /// <inheritdoc/>
    public EntityDescriptor ReadEntityDescriptor(
        XmlTraverser source,
        Action<ReadErrorInspectorContext<EntityDescriptor>>? errorInspector = null)
    {
        EntityDescriptor entityDescriptor = default!;

        if (source.EnsureName(Elements.EntityDescriptor, Namespaces.MetadataUri))
        {
            entityDescriptor = ReadEntityDescriptor(source);
        }

        source.MoveNext(true);

        // TODO: Test case for error inspector. Including test that it's not call on no errors.
        CallErrorInspector(errorInspector, entityDescriptor, source);

        ThrowOnErrors(source);

        return entityDescriptor;
    }

    /// <summary>
    /// Read an EntityDescriptor
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>EntityDescriptor</returns>
    protected EntityDescriptor ReadEntityDescriptor(XmlTraverser source)
    {
        var entityDescriptor = Create<EntityDescriptor>();

        ReadAttributes(source, entityDescriptor);
        ReadElements(source.GetChildren(), entityDescriptor);

        return entityDescriptor;
    }

    /// <summary>
    /// Read attributes of EntityDescriptor
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="entityDescriptor">EntityDescriptor</param>
    protected virtual void ReadAttributes(XmlTraverser source, EntityDescriptor entityDescriptor)
    {
        entityDescriptor.EntityId = source.GetRequiredAbsoluteUriAttribute(Attributes.entityID) ?? "";
        entityDescriptor.Id = source.GetAttribute(Attributes.ID);
        entityDescriptor.CacheDuraton = source.GetTimeSpanAttribute(Attributes.cacheDuration);
        entityDescriptor.ValidUntil = source.GetDateTimeAttribute(Attributes.validUntil);
    }

    /// <summary>
    /// Read the child elements of the EntityDescriptor.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="entityDescriptor">Entity Descriptor to populate</param>
    protected virtual void ReadElements(XmlTraverser source, EntityDescriptor entityDescriptor)
    {
        source.MoveNext();

        if (source.ReadAndValidateOptionalSignature(
            TrustedSigningKeys, AllowedHashAlgorithms, out var trustLevel))
        {
            entityDescriptor.TrustLevel = trustLevel;
            source.MoveNext();
        }

        if (source.HasName(Elements.Extensions, Namespaces.MetadataUri))
        {
            entityDescriptor.Extensions = ReadExtensions(source);
            source.MoveNext();
        }

        // Now we're at the actual role descriptors - or possibly an AffiliationDescriptor.
        bool wasRoleDescriptor = true; // Assume the best.
        do
        {
            if (source.EnsureNamespace(Namespaces.MetadataUri))
            {
                switch (source.CurrentNode?.LocalName)
                {
                    case Elements.RoleDescriptor:
                        entityDescriptor.RoleDescriptors.Add(ReadRoleDescriptor(source));
                        break;
                    case Elements.IDPSSODescriptor:
                        entityDescriptor.RoleDescriptors.Add(ReadIDPSSODescriptor(source));
                        break;
                    case Elements.SPSSODescriptor:
                    case Elements.AuthnAuthorityDescriptor:
                    case Elements.AttributeAuthorityDescriptor:
                    case Elements.PDPDescriptor:
                        source.IgnoreChildren();
                        break;
                    default:
                        wasRoleDescriptor = false; // Nope, something else.
                        break;
                }
            }
        } while (wasRoleDescriptor && source.MoveNext(true));

        // There can be more data after the role descriptors that we currently do not support, skip them.
        source.Skip();
    }
}