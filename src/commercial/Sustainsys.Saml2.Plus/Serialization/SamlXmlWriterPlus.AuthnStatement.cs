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
    /// Append an AuthnStatement element
    /// </summary>
    /// <param name="parent">Parent node</param>
    /// <param name="authnStatement">authnStatement</param>
    protected virtual void Append(XmlNode parent, AuthnStatement authnStatement)
    {
        var authnStatementElement = AppendElement(parent, Namespaces.Saml, Elements.AuthnStatement);

        authnStatementElement.SetAttribute(Attributes.AuthnInstant, authnStatement.AuthnInstant);
        authnStatementElement.SetAttributeIfValue(Attributes.SessionIndex, authnStatement.SessionIndex);

        if (authnStatement.AuthnContext != null)
        {
            var authnContextElement = AppendElement(authnStatementElement, Namespaces.Saml, Elements.AuthnContext);

            if (authnStatement.AuthnContext.AuthnContextClassRef != null)
            {
                var authnContextClassRefElement = AppendElement(authnContextElement, Namespaces.Saml, Elements.AuthnContextClassRef);
                authnContextClassRefElement.InnerText = authnStatement.AuthnContext.AuthnContextClassRef;
            }
        }
    }
}