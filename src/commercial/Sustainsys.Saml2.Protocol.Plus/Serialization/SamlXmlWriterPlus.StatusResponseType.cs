// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using System.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlWriterPlus
{
    /// <summary>
    /// Append a type derived from StatusResponseType, with the given name
    /// </summary>
    /// <param name="parent">Parent node to append child element to</param>
    /// <param name="statusResponseType">data</param>
    /// <param name="localName">Local name of the new element</param>
    protected virtual XmlElement Append(XmlNode parent, StatusResponseType statusResponseType, string localName)
    {
        var element = AppendElement(parent, Namespaces.Samlp, localName);
        element.SetAttribute(Attributes.ID, statusResponseType.Id);
        element.SetAttribute(Attributes.Version, statusResponseType.Version);
        element.SetAttribute(Attributes.IssueInstant, statusResponseType.IssueInstant);
        element.SetAttributeIfValue(Attributes.Destination, statusResponseType.Destination);
        element.SetAttributeIfValue(Attributes.InResponseTo, statusResponseType.InResponseTo);

        if (statusResponseType.Issuer != null)
        {
            Append(element, statusResponseType.Issuer, Elements.Issuer);
        }
        Append(element, statusResponseType.Status);

        return element;
    }
}