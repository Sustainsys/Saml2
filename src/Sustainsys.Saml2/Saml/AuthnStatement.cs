namespace Sustainsys.Saml2.Saml;

/// <summary>
/// AuthnStatement, Core 2.7.2
/// </summary>
public class AuthnStatement
{
    /// <summary>
    /// Authentication instant
    /// </summary>
    public DateTime AuthnInstant { get; set; }

    /// <summary>
    /// Session Index
    /// </summary>
    public string? SessionIndex { get; set; }

    /// <summary>
    /// A session established based on this assertion must expire by this time.
    /// </summary>
    public DateTime? SessionNotOnOrAfter { get; set; }

    /// <summary>
    /// Authentication Context
    /// </summary>
    public AuthnContext AuthnContext { get; set; } = default!;
}