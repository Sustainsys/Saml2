// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
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
    /// Append the descriptor as a child node
    /// </summary>
    /// <param name="node">parent node</param>
    /// <param name="entityDescriptor">Entity Descriptor</param>
    protected virtual XmlElement Append(XmlNode node, EntityDescriptor entityDescriptor)
    {
        var entityDescriptorElement = AppendElement(node, Namespaces.Metadata, Elements.EntityDescriptor);
        entityDescriptorElement.SetAttribute(Attributes.ID, entityDescriptor.Id);
        entityDescriptorElement.SetAttribute(Attributes.entityID, entityDescriptor.EntityId);
        entityDescriptorElement.SetAttributeIfValue(Attributes.cacheDuration, entityDescriptor.CacheDuration);
        entityDescriptorElement.SetAttributeIfValue(Attributes.validUntil, entityDescriptor.ValidUntil);

        foreach (var roleDescriptor in entityDescriptor.RoleDescriptors)
        {
            switch (roleDescriptor)
            {
                case IDPSSODescriptor idpSsoDescriptor:
                    Append(entityDescriptorElement, idpSsoDescriptor);
                    break;
            }
        }

        return entityDescriptorElement;
    }
}