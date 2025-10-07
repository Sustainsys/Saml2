// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Saml;
using System.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlWriterPlus
{
    /// <summary>
    /// Append a NameId
    /// </summary>
    /// <param name="parent">Parent node to append child element to</param>
    /// <param name="issuer">value</param>
    protected virtual void Append(XmlNode parent, NameId? issuer)
    {
        if (issuer != null)
        {
            var issuerElement = Append(parent, Namespaces.Saml, Elements.Issuer);
            issuerElement.InnerText = issuer.Value;
            issuerElement.SetAttribute(Attributes.Format, issuer.Format);
        }
    }
}
