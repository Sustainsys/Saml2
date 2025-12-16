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

    /// <summary>
    /// Append the service as a child node
    /// </summary>
    /// <param name="node">parent node</param>
    /// <param name="service">Single Sign On Service</param>
    protected virtual XmlElement Append(XmlNode node, Endpoint service)
    {
        var serviceElement = null as XmlElement;
        switch (service.Type)
        {
            case Elements.SingleSignOnService:
                serviceElement = AppendElement(node, Namespaces.Metadata, Elements.SingleSignOnService);
                break;
            case Elements.SingleLogoutService:
                serviceElement = AppendElement(node, Namespaces.Metadata, Elements.SingleLogoutService);
                break;
            default:
                throw new ArgumentException($"Unknown service type: {service.Type}");
        }
        serviceElement.SetAttribute(Attributes.Binding, service.Binding);
        serviceElement.SetAttribute(Attributes.Location, service.Location);

        node.AppendChild(serviceElement);

        return serviceElement;
    }
}