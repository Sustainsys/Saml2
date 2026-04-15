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
    /// Append the descriptor as a child node
    /// </summary>
    /// <param name="node">parent node</param>
    /// <param name="idpSsoDescriptor">IDPSSO Descriptor</param>
    protected virtual XmlElement Append(XmlNode node, IDPSSODescriptor idpSsoDescriptor)
    {
        var idpSsoDescriptorElement = AppendElement(node, Namespaces.Metadata, Elements.IDPSSODescriptor);
        idpSsoDescriptorElement.SetAttribute(Attributes.protocolSupportEnumeration, idpSsoDescriptor.ProtocolSupportEnumeration);

        foreach (var singleLogoutService in idpSsoDescriptor.SingleLogoutServices)
        {
            Append(idpSsoDescriptorElement, singleLogoutService, Elements.SingleLogoutService);
        }
        foreach (var singleSignOnService in idpSsoDescriptor.SingleSignOnServices)
        {
            Append(idpSsoDescriptorElement, singleSignOnService, Elements.SingleSignOnService);
        }

        return idpSsoDescriptorElement;
    }
}