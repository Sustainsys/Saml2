using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlReader
{
    /// <inheritdoc/>
    public SamlResponse ReadSamlResponse(
        XmlTraverser source,
        Action<ReadErrorInspectorContext<SamlResponse>>? errorInspector = null)
    {
        SamlResponse samlResponse = default!;

        if (source.EnsureName(Constants.Namespaces.SamlpUri, Constants.Elements.Response))
        {
            samlResponse = ReadSamlResponse(source);
        }

        source.MoveNext(true);

        // TODO: Test for error inspector call
        CallErrorInspector(errorInspector, samlResponse, source);

        source.ThrowOnErrors();

        return samlResponse;
    }

    /// <summary>
    /// Read a Saml Response
    /// </summary>
    /// <param name="source">Source Data</param>
    /// <returns>SamlResponse</returns>
    protected virtual SamlResponse ReadSamlResponse(XmlTraverser source)
    {
        var samlResponse = Create<SamlResponse>();

        ReadAttributes(source, samlResponse);
        ReadElements(source.GetChildren(), samlResponse);

        return samlResponse;
    }

    /// <summary>
    /// Read elements of SamlResponse
    /// </summary>
    /// <param name="source">XmlTraverser</param>
    /// <param name="samlResponse">SamlResponse to populate</param>
    protected virtual void ReadElements(XmlTraverser source, SamlResponse samlResponse)
    {
        ReadElements(source, (StatusResponseType)samlResponse);

        while (source.HasName(Namespaces.SamlUri, Elements.Assertion))
        {
            samlResponse.Assertions.Add(ReadAssertion(source));
            source.MoveNext(true);
        }
    }
}
