using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sustainsys.Saml2.Samlp;
partial class SamlpSerializer
{
    /// <inheritdoc/>
    public AuthnRequest ReadAuthnRequest(XmlTraverser source)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual XmlDocument Write(AuthnRequest authnRequest)
    {
        var xmlDoc = CreateXmlDocument();

        Append(xmlDoc, authnRequest);

        return xmlDoc;
    }

    /// <summary>
    /// Append the authnrequest as a child node
    /// </summary>
    /// <param name="node">parent node</param>
    /// <param name="authnRequest">AuthnRequest</param>
    protected virtual void Append(XmlNode node, AuthnRequest authnRequest)
    {
        var xe = Append(node, authnRequest, "AuthnRequest");
        xe.SetAttributeIfValue("AssertionConsumerServiceURL", authnRequest.AssertionConsumerServiceUrl);
        samlSerializer.AppendIfValue(xe, authnRequest.Issuer, "Issuer");
    }
}
