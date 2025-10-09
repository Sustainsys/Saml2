// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using System.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlWriterPlus
{
    /// <summary>
    /// Append an Assertion element
    /// </summary>
    /// <param name="parent">Parent node</param>
    /// <param name="assertion">Saml assertion</param>
    /// <returns>XmlElement</returns>
    protected virtual XmlElement Append(XmlNode parent, Assertion assertion)
    {
        var element = AppendElement(parent, Namespaces.Saml, Elements.Assertion);
        element.SetAttribute(Attributes.ID, assertion.Id);
        element.SetAttribute(Attributes.Version, assertion.Version);
        element.SetAttribute(Attributes.IssueInstant, assertion.IssueInstant);

        Append(element, assertion.Issuer, "Issuer");

        if (assertion.Subject != null)
        {
            Append(element, assertion.Subject);
        }

        if (assertion.Conditions != null)
        {
            Append(element, assertion.Conditions);
        }

        if (assertion.AuthnStatement != null)
        {
            Append(element, assertion.AuthnStatement);
        }
        Append(element, assertion.Attributes);

        return element;
    }
}