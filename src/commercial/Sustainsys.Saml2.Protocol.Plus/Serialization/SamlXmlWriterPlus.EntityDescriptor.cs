// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Samlp;
using System.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlWriterPlus
{
    /// <inheritdoc/>
    public virtual XmlDocument Write(EntityDescriptor entityDescriptor)
    {
        var xmlDoc = new XmlDocument();

        Append(xmlDoc, entityDescriptor);

        return xmlDoc;
    }

    /// <summary>
    /// Append the response as a child node
    /// </summary>
    /// <param name="node">parent node</param>
    /// <param name="entityDescriptor">Entity Descriptor</param>
    protected virtual XmlElement Append(XmlNode node, EntityDescriptor entityDescriptor)
    {
        var entityDescriptorElement = AppendElement(
            node, Namespaces.Metadata, Elements.EntityDescriptor);

        return entityDescriptorElement;
    }
}