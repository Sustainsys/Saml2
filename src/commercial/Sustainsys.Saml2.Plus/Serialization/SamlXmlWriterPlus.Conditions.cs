// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Saml;
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
        if (conditions != null)
        {
            var conditionsElement = AppendElement(parent, Namespaces.Saml, Elements.Conditions);
            if (conditions.NotOnOrAfter.HasValue)
            {
                var dt = new DateTime(conditions.NotOnOrAfter.Value.Ticks, DateTimeKind.Utc);
                conditionsElement.SetAttribute(Attributes.NotOnOrAfter, dt.ToString("yyyy-MM-ddTHH:mm:ss\\Z"));
            }

            var audienceRestrictionElement = AppendElement(conditionsElement, Namespaces.Saml, Elements.AudienceRestriction);
            var audienceElement = AppendElement(audienceRestrictionElement, Namespaces.Saml, Elements.Audience);
            audienceElement.InnerText = "https://sp.example.com/Saml2";
        }
    }
}