namespace Sustainsys.Saml2.Saml;

/// <summary>
/// The Saml NameIDType
/// </summary>
public class NameId
{
    /// <summary>
    /// The value, i.e. string contents of the XML node.
    /// </summary>
    public string Value { get; set; } = default!;

    /// <summary>
    /// Implicit operator creating a NameId with the string supplied as value.
    /// </summary>
    /// <param name="value">The value of the NameId</param>
    public static implicit operator NameId(string value) => new() { Value = value };

    /// <summary>
    /// Shows debugger friendly string
    /// </summary>
    /// <returns>string</returns>
    public override string ToString() => $"{{{Value}}}";
}
