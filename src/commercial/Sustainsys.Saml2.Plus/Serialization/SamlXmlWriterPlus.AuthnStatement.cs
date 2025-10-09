// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Saml;
using System.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlWriterPlus
{
    /// <summary>
    /// Append an AuthnStatement element
    /// </summary>
    /// <param name="parent">Parent node</param>
    /// <param name="authnStatement">authnStatement</param>
    protected virtual void Append(XmlNode parent, AuthnStatement authnStatement)
    {
        var authnStatementElement = AppendElement(parent, Namespaces.Saml, Elements.AuthnStatement);

        var dt = new DateTime(authnStatement.AuthnInstant.Ticks, DateTimeKind.Utc);
        authnStatementElement.SetAttribute(Attributes.AuthnInstant, dt.ToString("yyyy-MM-ddTHH:mm:ss\\Z"));
        authnStatementElement.SetAttribute(Attributes.SessionIndex, authnStatement.SessionIndex);

        var authnContextElement = AppendElement(authnStatementElement, Namespaces.Saml, Elements.AuthnContext);
        var authnContextClassRefElement = AppendElement(authnContextElement, Namespaces.Saml, Elements.AuthnContextClassRef);
        authnContextClassRefElement.InnerText = "urn:oasis:names:tc:SAML:2.0:ac:classes:unspecified";
    }
}