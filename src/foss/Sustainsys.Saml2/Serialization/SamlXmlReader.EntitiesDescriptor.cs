// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlReader
{
    /// <inheritdoc/>
    public EntitiesDescriptor ReadEntitiesDescriptor(
        XmlTraverser source,
        Action<ReadErrorInspectorContext<EntitiesDescriptor>>? errorInspector = null)
    {
        EntitiesDescriptor entitiesDescriptor = default!;
        if (source.EnsureName(Elements.EntitiesDescriptor, Namespaces.MetadataUri))
        {
            entitiesDescriptor = ReadEntitiesDescriptor(source);
        }

        source.MoveNext(true);

        // TODO: Test case for error inspector. Including test that it's not call on no errors.
        CallErrorInspector(errorInspector, entitiesDescriptor, source);

        ThrowOnErrors(source);

        return entitiesDescriptor;
    }

    /// <summary>
    /// Read an EntitiesDescriptor
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>EntitiesDescriptor</returns>
    protected EntitiesDescriptor ReadEntitiesDescriptor(XmlTraverser source)
    {
        var entitiesDescriptor = Create<EntitiesDescriptor>();

        ReadAttributes(source, entitiesDescriptor);
        ReadElements(source.GetChildren(), entitiesDescriptor);

        return entitiesDescriptor;
    }

    /// <summary>
    /// Read attributes of EntitiesDescriptor
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="entitiesDescriptor">EntitiesDescriptor</param>
    protected virtual void ReadAttributes(XmlTraverser source, EntitiesDescriptor entitiesDescriptor)
    {
        entitiesDescriptor.Id = source.GetAttribute(Attributes.ID);
        entitiesDescriptor.CacheDuration = source.GetTimeSpanAttribute(Attributes.cacheDuration);
        entitiesDescriptor.ValidUntil = source.GetDateTimeAttribute(Attributes.validUntil);
    }

    /// <summary>
    /// Read the child elements of the EntitiesDescriptor.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="entitiesDescriptor">EntitiesDescriptor to populate</param>
    protected virtual void ReadElements(XmlTraverser source, EntitiesDescriptor entitiesDescriptor)
    {
        source.MoveNext(false);

        // TODO: Use EntityResolver instead of properties on the reader.

        if (source.ReadAndValidateOptionalSignature(
            TrustedSigningKeys, AllowedAlgorithms))
        {
            entitiesDescriptor.TrustLevel = source.TrustLevel;
            source.MoveNext(false);
        }

        if (source.HasName(Elements.Extensions, Namespaces.MetadataUri))
        {
            entitiesDescriptor.Extensions = ReadExtensions(source);
            source.MoveNext(false);
        }

        // Now we're at the actual entity descriptors - or possibly another EntitiesDescriptor.
        bool wasEntityDescriptor = true; // Assume the best.
        do
        {
            if (source.EnsureNamespace(Namespaces.MetadataUri))
            {
                switch (source.CurrentNode?.LocalName)
                {
                    case Elements.EntityDescriptor:
                        entitiesDescriptor.EntityDescriptors.Add(ReadEntityDescriptor(source));
                        break;
                    // TODO: Support nested EntitiesDescriptor.
                    case Elements.EntitiesDescriptor:
                        source.IgnoreChildren();
                        break;
                    default:
                        wasEntityDescriptor = false; // Nope, something else.
                        break;
                }
            }
        } while (wasEntityDescriptor && source.MoveNext(true));

        // There can be more data after the entity descriptors that we currently do not support, skip them.
        source.Skip();
    }
}