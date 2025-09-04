namespace Sustainsys.Saml2.Saml;

/// <summary>
/// The Saml NameIDType
/// </summary>
public class NameId
{

    /// <summary>
    /// Ctor
    /// </summary>
    public NameId() { }

    /// <summary>
    /// Ctor
    /// </summary>
    public NameId(string value, string? format = null)
    {
        Value = value;
        Format = format;
    }

    /// <summary>
    /// A URI reference representing the classification of string-based identifier information. 
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// The value, i.e. string contents of the XML node.
    /// </summary>
    public string Value { get; set; } = default!;

    /// <summary>
    /// Implicit operator creating a NameId with the string supplied as value.
    /// </summary>
    /// <param name="value">The value of the NameId</param>
    public static implicit operator NameId(string value) => new(value);

    /// <summary>
    /// Shows debugger friendly string
    /// </summary>
    /// <returns>string</returns>
    public override string ToString() => Value;

    /// <summary>
    /// Comparison
    /// </summary>
    /// <param name="other">Object to compare to</param>
    /// <returns>Are they equal?</returns>
    public override bool Equals(object? other)
    =>
    (other is NameId othernameid
    && othernameid.Value == Value && othernameid.Format == Format)
    ||
    (other is string otherstring
    && otherstring == Value && Format == null);

    /// <summary>
    /// Operator ==
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    public static bool operator ==(NameId? n1, NameId? n2) =>
        n1?.Value == n2?.Value && n1?.Format == n2?.Format;

    /// <summary>
    /// Operator !=
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    public static bool operator !=(NameId? n1, NameId? n2) =>
        n1?.Value != n2?.Value && n1?.Format == n2?.Format;

    /// <summary>
    /// Get hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode() =>
        Value.GetHashCode();
}