namespace Sustainsys.Saml2.Samlp;

/// <summary>
/// Samlp Status element
/// </summary>
public class SamlStatus
{
    /// <summary>
    /// Status Code
    /// </summary>
    public StatusCode StatusCode { get; set; } = default!;
}

/// <summary>
/// Samlp StatusCode element
/// </summary>
public class StatusCode
{
    /// <summary>
    /// Status code value.
    /// </summary>
    public string Value { get; set; } = default!;
}
