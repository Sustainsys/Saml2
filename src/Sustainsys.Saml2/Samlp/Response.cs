using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.Samlp;

/// <summary>
/// A Saml2p SamlResponse
/// </summary>
public class Response : StatusResponseType
{
    /// <summary>
    /// Assertions
    /// </summary>
    public List<Assertion> Assertions { get; } = [];
}