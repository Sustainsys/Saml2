namespace Sustainsys.Saml2.Saml;

/// <summary>
/// Audience Restrictions, Core 2.5.1.4
/// </summary>
public class AudienceRestriction
{
    /// <summary>
    /// Audiences, a list of URIs identifying the audiences.
    /// </summary>
    public List<string> Audiences { get; } = [];
}