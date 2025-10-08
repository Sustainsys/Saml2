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
    /// <param name="parent"></param>
    /// <param name="assertion"></param>
    /// <returns></returns>
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
        if (assertion.Attributes != null)
        {
            Append(element, assertion.Attributes);
        }

        return element;
    }
}