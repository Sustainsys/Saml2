// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.Samlp;
using System.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlWriterPlus
{
    /// <inheritdoc/>
    public virtual XmlDocument Write(Response response)
    {
        var xmlDoc = new XmlDocument();

        Append(xmlDoc, response);

        return xmlDoc;
    }

    /// <summary>
    /// Append the response as a child node
    /// </summary>
    /// <param name="node">parent node</param>
    /// <param name="response">Response</param>
    protected virtual XmlElement Append(XmlNode node, Response response)
    {
        var responseElement = Append(node, response, Elements.Response);

        foreach (var assertion in response.Assertions)
        {
            Append(responseElement, assertion);
        }

        return responseElement;
    }
}