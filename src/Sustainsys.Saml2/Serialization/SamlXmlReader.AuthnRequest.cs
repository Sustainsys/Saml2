using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
public partial class SamlXmlReader
{
    // TODO: Convert other reads to follow this pattern with a callback for errors

    /// <inheritdoc/>
    public AuthnRequest ReadAuthnRequest(
        XmlTraverser source,
        Action<ReadErrorInspectorContext<AuthnRequest>>? errorInspector = null)
    {
        AuthnRequest authnRequest = default!;

        if (source.EnsureName(Elements.AuthnRequest, Namespaces.SamlpUri))
        {
            authnRequest = ReadAuthnRequest(source);
            source.MoveNext(true);
        }

        CallErrorInspector(errorInspector, authnRequest, source);

        source.ThrowOnErrors();

        return authnRequest;
    }

    /// <summary>
    /// Read an <see cref="AuthnRequest"/>
    /// </summary>
    /// <param name="source">Xml Traverser to read from</param>
    /// <returns><see cref="AuthnRequest"/>The AuthnRequest read</returns>
    protected AuthnRequest ReadAuthnRequest(XmlTraverser source)
    {
        var authnRequest = Create<AuthnRequest>();

        ReadAttributes(source, authnRequest);
        ReadElements(source.GetChildren(), authnRequest);

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

        if (source.HasName(Elements.Subject, Namespaces.SamlUri))
        {
            authnRequest.Subject = ReadSubject(source);
            source.MoveNext(true);
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

        authnRequest.ForceAuthn = source.GetBoolAttribute(Attributes.ForceAuthn) ?? authnRequest.ForceAuthn;
        authnRequest.IsPassive = source.GetBoolAttribute(Attributes.IsPassive) ?? authnRequest.IsPassive;
        authnRequest.AssertionConsumerServiceIndex = source.GetIntAttribute(Attributes.AssertionConsumerServiceIndex);
        authnRequest.AssertionConsumerServiceUrl = source.GetAttribute(Attributes.AssertionConsumerServiceURL);
        authnRequest.ProtocolBinding = source.GetAttribute(Attributes.ProtocolBinding);
    }
}
