// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlReader
{
    /// <summary>
    /// Reads an endpoint.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <returns>Endpoint read</returns>
    protected Endpoint ReadEndpoint(XmlTraverser source)
    {
        var result = Create<Endpoint>();
        ReadAttributes(source, result);

        return result;
    }

    /// <summary>
    /// Read endpoint attributes.
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="endpoint">Endpoint</param>
    protected virtual void ReadAttributes(XmlTraverser source, Endpoint endpoint)
    {
        endpoint.Binding = source.GetRequiredAbsoluteUriAttribute(Attributes.Binding) ?? "";
        endpoint.Location = source.GetRequiredAttribute(Attributes.Location) ?? "";
    }

    /// <summary>
    /// Read indexed endpoint
    /// </summary>
    /// <param name="source">Source</param>
    /// <returns>IndexedEndpoint</returns>
    protected virtual IndexedEndpoint ReadIndexedEndpoint(XmlTraverser source)
    {
        var result = Create<IndexedEndpoint>();
        result.Index = source.GetRequiredIntAttribute(Attributes.index);
        result.IsDefault = source.GetBoolAttribute(Attributes.isDefault) ?? false;

        ReadAttributes(source, result);

        return result;
    }
}