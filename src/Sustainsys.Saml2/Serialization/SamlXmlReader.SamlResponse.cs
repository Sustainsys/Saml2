using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlReader
{
    /// <summary>
    /// Create an empty saml response instance.
    /// </summary>
    /// <returns>SamlResponse</returns>
    protected virtual SamlResponse CreateSamlResponse() => new();

	/// <inheritdoc/>
	public virtual SamlResponse ReadSamlResponse(XmlTraverser source)
	{
        var samlResponse = CreateSamlResponse();

        if(source.EnsureName(Constants.Namespaces.SamlpUri, Constants.Elements.Response))
        {
            ReadAttributes(source, samlResponse);
            ReadElements(source.GetChildren(), samlResponse);
        }

        source.MoveNext(true);

        source.ThrowOnErrors();

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
    }
}
