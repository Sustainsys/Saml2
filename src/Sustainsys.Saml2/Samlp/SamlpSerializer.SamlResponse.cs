using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.Samlp;

partial class SamlpSerializer
{
	/// <inheritdoc/>
	public TrustedData<SamlResponse> ReadSamlResponse(XmlTraverser source)
	{
        return new TrustedData<SamlResponse>(TrustLevel.None, new SamlResponse());
	}
}
