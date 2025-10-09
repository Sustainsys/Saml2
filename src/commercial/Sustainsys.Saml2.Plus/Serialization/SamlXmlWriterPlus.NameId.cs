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
    /// Append a NameId
    /// </summary>
    /// <param name="parent">Parent node to append child element to</param>
    /// <param name="nameId">value</param>
    /// <param name="localName">Local name of the new element</param>
    protected virtual XmlElement Append(XmlNode parent, NameId? nameId, string localName)
    {
        var element = AppendElement(parent, Namespaces.Saml, localName);
        if (nameId != null)
        {
            element.InnerText = nameId.Value;
            element.SetAttributeIfValue(Attributes.Format, nameId.Format);
        }

        return element;
    }
}