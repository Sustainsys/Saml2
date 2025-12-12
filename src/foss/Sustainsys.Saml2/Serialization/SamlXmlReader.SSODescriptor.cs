// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlReader
{
    /// <summary>
    /// Read attributes of SSODescriptor.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="result">Target to set properties on</param>
    protected virtual void ReadAttributes(XmlTraverser source, SSODescriptor result)
    {
        ReadAttributes(source, (RoleDescriptor)result);
    }

    /// <summary>
    /// Read elements of SSODescriptor
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="result">Target to set properties on</param>
    protected virtual void ReadElements(XmlTraverser source, SSODescriptor result)
    {
        ReadElements(source, (RoleDescriptor)result);

        while (source.HasName(Elements.ArtifactResolutionService, Namespaces.MetadataUri))
        {
            result.ArtifactResolutionServices.Add(ReadIndexedEndpoint(source));

            source.MoveNext(true);
        }

        while (source.HasName(Elements.SingleLogoutService, Namespaces.MetadataUri))
        {
            result.SingleLogoutServices.Add(ReadEndpoint(source));

            source.MoveNext(true);
        }

        while (source.HasName(Elements.ManageNameIDService, Namespaces.MetadataUri)
            || source.HasName(Elements.NameIDFormat, Namespaces.MetadataUri))
        {
            // We're not supporting ManageNameIDService nor NameIDFormat.
            source.IgnoreChildren();

            source.MoveNext(true);
        }
    }
}