// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Samlp;
using System.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlWriterPlus
{
    /// <summary>
    /// Append a Saml status
    /// </summary>
    /// <param name="parent">Parent node to append child element to</param>
    /// <param name="status">value</param>
    protected virtual void Append(XmlNode parent, SamlStatus status)
    {
        var statusElement = AppendElement(parent, Namespaces.Samlp, Elements.Status);

        var statusCodeElement = AppendElement(statusElement, Namespaces.Samlp, Elements.StatusCode);
        statusCodeElement.SetAttribute(Attributes.Value, status.StatusCode.Value);
    }
}