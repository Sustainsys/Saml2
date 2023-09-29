using Sustainsys.Saml2.Metadata.Xml;
using Sustainsys.Saml2.Saml;
using System.Reflection.Metadata.Ecma335;
using System.Xml;

namespace Sustainsys.Saml2.Samlp;

/// <summary>
/// Serializer for Saml protocol classes
/// </summary>
public interface ISamlpSerializer
{
    /// <summary>
    /// Create an Xml document and write an AuthnRequest to it.
    /// </summary>
    /// <param name="authnRequest">AuthnRequest</param>
    /// <returns>Created XmlDoc</returns>
    XmlDocument Write(AuthnRequest authnRequest);

    /// <summary>
    /// Read an AuthnRequest from Xml
    /// </summary>
    /// <param name="source">XmlTraverser to read from</param>
    /// <returns>AuthnRequest</returns>
    public AuthnRequest ReadAuthnRequest(XmlTraverser source);
}

/// <summary>
/// Serializer for Saml protocol classes
/// </summary>
public class SamlpSerializer : SerializerBase, ISamlpSerializer
{
    private readonly ISamlSerializer samlSerializer;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="samlSerializer"></param>
    public SamlpSerializer(ISamlSerializer samlSerializer)
    {
        Prefix = "samlp";
        NamespaceUri = Constants.SamlpNamespace;
        this.samlSerializer = samlSerializer;
    }

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
        Append(node, authnRequest, "AuthnRequest");
    }

    /// <summary>
    /// Append an type derived from RequestAbstractType, with the given name.
    /// </summary>
    /// <param name="parent">Parent node to append child element to</param>
    /// <param name="request">data</param>
    /// <param name="localName">Local name of the new element.</param>
    protected virtual XmlElement Append(XmlNode parent, RequestAbstractType request, string localName)
    {
        var element = Append(parent, localName);
        element.SetAttribute("ID", request.Id);
        element.SetAttribute("IssueInstant", XmlConvert.ToString(request.IssueInstant));
        element.SetAttribute("Version", request.Version);

        return element;
    }
}
