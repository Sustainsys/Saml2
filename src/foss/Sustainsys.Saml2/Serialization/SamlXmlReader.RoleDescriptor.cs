// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Xml;
using System.Security.Cryptography.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlReader
{
    /// <summary>
    /// Process a RoleDescriptor element.
    /// </summary>
    /// <param name="source">Source</param>
    /// <returns>True if current node was a RoleDescriptor element</returns>
    protected virtual RoleDescriptor ReadRoleDescriptor(XmlTraverser source)
    {
        var result = Create<RoleDescriptor>();

        ReadAttributes(source, result);
        ReadElements(source.GetChildren(), result);

        // Custom RoleDesciptors might have other elements that we do not know - ignore them.
        source.IgnoreChildren();

        return result;
    }

    /// <summary>
    /// Read attributs of RoleDescriptor
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="result">Target to set properties on</param>
    protected virtual void ReadAttributes(XmlTraverser source, RoleDescriptor result)
    {
        result.ProtocolSupportEnumeration =
            source.GetRequiredAbsoluteUriAttribute(Attributes.protocolSupportEnumeration);
    }

    /// <summary>
    /// Read elements of RoleDescriptor
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="result">Target to set properties on</param>
    /// <returns>More elements available?</returns>
    protected virtual void ReadElements(XmlTraverser source, RoleDescriptor result)
    {
        source.MoveNext(true);

        if (source.HasName(Elements.Signature, SignedXml.XmlDsigNamespaceUrl))
        {
            // Signatures on RoleDescriptors are not supported.
            source.IgnoreChildren();

            source.MoveNext(true);
        }

        if (source.HasName(Elements.Extensions, Namespaces.MetadataUri))
        {
            // Extensions on RoleDescriptors are not supported.
            source.IgnoreChildren();

            source.MoveNext(true);
        }

        while (source.HasName(Elements.KeyDescriptor, Namespaces.MetadataUri))
        {
            result.Keys.Add(ReadKeyDescriptor(source));
            source.MoveNext(true);
        }

        if (source.HasName(Elements.Organization, Namespaces.MetadataUri))
        {
            // Organization reading is not supported.
            source.IgnoreChildren();

            source.MoveNext(true);
        }

        if (source.HasName(Elements.ContactPerson, Namespaces.MetadataUri))
        {
            // Contact person reading is not supported.
            source.IgnoreChildren();

            source.MoveNext(true);
        }
    }
}