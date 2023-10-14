using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.Samlp;

partial class SamlpSerializer
{
    /// <summary>
    /// Create an empty saml response instance.
    /// </summary>
    /// <returns>SamlResponse</returns>
    protected virtual SamlResponse CreateSamlResponse() => new();

	/// <inheritdoc/>
	public TrustedData<SamlResponse> ReadSamlResponse(XmlTraverser source)
	{
        var samlResponse = CreateSamlResponse();

        return new TrustedData<SamlResponse>(TrustLevel.None, samlResponse);
	}
}
