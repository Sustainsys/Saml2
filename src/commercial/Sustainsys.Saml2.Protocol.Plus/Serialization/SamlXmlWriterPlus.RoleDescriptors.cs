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
    /// <summary>
    /// Append the service as a child node
    /// </summary>
    /// <param name="node">parent node</param>
    /// <param name="service">Service that will become child node</param>
    /// <param name="elementName">Name of the element</param>
    protected virtual XmlElement Append(XmlNode node, Endpoint service, string elementName)
    {
        var serviceElement = AppendElement(node, Namespaces.Metadata, elementName);

        serviceElement.SetAttribute(Attributes.Binding, service.Binding);
        serviceElement.SetAttribute(Attributes.Location, service.Location);

        return serviceElement;
    }
}