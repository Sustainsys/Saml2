namespace Sustainsys.Saml2.Saml;

/// <summary>
/// Saml Attribute, Core 2.7.3.1
/// </summary>
public class SamlAttribute
{
    /// <summary>
    /// Name of attribute
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Attribute values.
    /// </summary>
    public List<string?> Values { get; } = [];
}