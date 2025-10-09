// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Saml;
using System.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlWriterPlus
{
    /// <summary>
    /// Append an AttributeStatement element
    /// </summary>
    /// <param name="parent">Parent node</param>
    /// <param name="attributeStatement">data</param>
    protected virtual XmlElement Append(XmlNode parent, AttributeStatement attributeStatement)
    {
        var statementElement = AppendElement(parent, Namespaces.Saml, Elements.AttributeStatement);
        foreach (var attribute in attributeStatement)
        {
            var attributeElement = AppendElement(statementElement, Namespaces.Saml, Elements.Attribute);
            attributeElement.SetAttribute("Name", attribute.Name);
            foreach (var value in attribute.Values)
            {
                var valueElement = AppendElement(attributeElement, Namespaces.Saml, Elements.AttributeValue);

                if (value == null)
                {
                    valueElement.SetAttribute("nil", Namespaces.XsiUri, "true");
                }
                else
                {
                    if (value != "")
                    {
                        valueElement.InnerText = value;
                    }
                }
            }
        }

        return statementElement;
    }
}