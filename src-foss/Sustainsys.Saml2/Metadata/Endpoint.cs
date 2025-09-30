namespace Sustainsys.Saml2.Metadata;

/// <summary>
/// Saml2 Endpoint Type.
/// </summary>
public class Endpoint
{
    /// <summary>
    /// Binding supported by the endpoint.
    /// </summary>
    public string Binding { get; set; } = "";

    /// <summary>
    /// URL of the endpoint.
    /// </summary>
    public string Location { get; set; } = "";
}