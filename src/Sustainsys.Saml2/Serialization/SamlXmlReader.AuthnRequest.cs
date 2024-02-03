using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using System.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
public partial class SamlXmlReader
{
    /// <summary>
    /// Create an empty AuthnRequest instance
    /// </summary>
    /// <returns>AuthnRequest</returns>
    protected virtual AuthnRequest CreateAuthnRequest() => new();

    //TODO: Convert other reads to follow this pattern with a callback for errors

    /// <inheritdoc/>
    public AuthnRequest ReadAuthnRequest(
        XmlTraverser source,
        Action<ReadErrorInspectorContext<AuthnRequest>>? errorInspector = null)
    {
        var authnRequest = ReadAuthnRequest(source);

        CallErrorInspector(errorInspector, authnRequest, source);

        source.ThrowOnErrors();

        return authnRequest;
    }

    /// <summary>
    /// Read an <see cref="AuthnRequest"/>
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <returns><see cref="AuthnRequest"/>The AuthnRequest read</returns>
    protected virtual AuthnRequest ReadAuthnRequest(XmlTraverser source)
    {
        var authnRequest = CreateAuthnRequest();

        if (source.EnsureName(Namespaces.SamlpUri, Elements.AuthnRequest))
        {
            ReadAttributes(source, authnRequest);
            ReadElements(source.GetChildren(), authnRequest);
        }

        source.MoveNext(true);

        return authnRequest;
    }

    /// <summary>
    /// Reads the child elements of an AuthnRequest.
    /// </summary>
    /// <param name="source">Xml traverser to read from</param>
    /// <param name="authnRequest">AuthnRequest to pupulate</param>
    protected virtual void ReadElements(XmlTraverser source, AuthnRequest authnRequest)
    {
        ReadElements(source, (RequestAbstractType)authnRequest);

        if (source.HasName(Namespaces.SamlUri, Elements.Subject))
        {
            authnRequest.Subject = ReadSubject(source);
        }
    }

    /// <summary>
    /// Reads attributes of an AuthnRequest
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <param name="authnRequest">The AuthnRequest to populate</param>
    protected virtual void ReadAttributes(XmlTraverser source, AuthnRequest authnRequest)
    {
        ReadAttributes(source, (RequestAbstractType)authnRequest);

        authnRequest.ForceAuthn = source.GetBoolAttribute(AttributeNames.ForceAuthn) ?? authnRequest.ForceAuthn;
        authnRequest.IsPassive = source.GetBoolAttribute(AttributeNames.IsPassive) ?? authnRequest.IsPassive;
        authnRequest.AssertionConsumerServiceIndex = source.GetIntAttribute(AttributeNames.AssertionConsumerServiceIndex);
        authnRequest.AssertionConsumerServiceUrl = source.GetAttribute(AttributeNames.AssertionConsumerServiceURL);
        authnRequest.ProtocolBinding = source.GetAttribute(AttributeNames.ProtocolBinding);
    }
}
