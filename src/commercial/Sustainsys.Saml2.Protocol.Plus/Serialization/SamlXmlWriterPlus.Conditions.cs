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
    /// Append a Conditions element
    /// </summary>
    /// <param name="parent">Parent node</param>
    /// <param name="conditions">value</param>
    protected virtual void Append(XmlNode parent, Conditions conditions)
    {
        var conditionsElement = AppendElement(parent, Namespaces.Saml, Elements.Conditions);

        conditionsElement.SetAttributeIfValue(Attributes.NotOnOrAfter, conditions.NotOnOrAfter);

        foreach (var restriction in conditions.AudienceRestrictions)
        {
            var audienceRestrictionElement = AppendElement(conditionsElement, Namespaces.Saml, Elements.AudienceRestriction);
            foreach (var audience in restriction.Audiences)
            {
                var audienceElement = AppendElement(audienceRestrictionElement, Namespaces.Saml, Elements.Audience);
                audienceElement.InnerText = audience;
            }
        }
    }
}