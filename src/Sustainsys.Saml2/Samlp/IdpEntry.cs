using System.Globalization;

namespace Sustainsys.Saml2.Samlp;
/// <summary>
/// Specifies a single identity provider.
/// </summary>
public class IdpEntry
{
    /// <summary>
    /// Unique identifier of the identity provider.
    /// </summary>
    public string ProviderId { get; set; } = default!;

    /// <summary>
    /// Name for the identity provider.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// URI reference.
    /// </summary>
    public string? Loc { get; set; }


}