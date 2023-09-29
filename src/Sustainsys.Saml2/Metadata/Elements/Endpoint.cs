using Sustainsys.Saml2.Metadata.Attributes;

namespace Sustainsys.Saml2.Metadata.Elements;

/// <summary>
/// Saml2 Endpoint Type.
/// </summary>
public class Endpoint
{
    /// <summary>
    /// Binding supported by the endpoint.
    /// </summary>
    public Binding Binding { get; set; }

    /// <summary>
    /// URL of the endpoint.
    /// </summary>
    public string Location { get; set; } = "";
}