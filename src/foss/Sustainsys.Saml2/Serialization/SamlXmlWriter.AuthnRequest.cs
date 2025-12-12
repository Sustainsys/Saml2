// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using System.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlWriter
{
    /// <inheritdoc/>
    public virtual XmlDocument Write(AuthnRequest authnRequest)
    {
        var xmlDoc = new XmlDocument();

        Append(xmlDoc, authnRequest);

        return xmlDoc;
    }

    /// <summary>
    /// Append the authnrequest as a child node
    /// </summary>
    /// <param name="node">parent node</param>
    /// <param name="authnRequest">AuthnRequest</param>
    protected virtual void Append(XmlNode node, AuthnRequest authnRequest)
    {
        var xe = Append(node, authnRequest, Elements.AuthnRequest);
        xe.SetAttributeIfValue(Attributes.AssertionConsumerServiceURL, authnRequest.AssertionConsumerServiceUrl);
        AppendIfValue(xe, authnRequest.Issuer, Namespaces.Saml, Elements.Issuer);
    }
}