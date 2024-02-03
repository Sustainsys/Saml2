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
    /// <param name="value">NameId value</param>
    public NameId(string value)
    {
        Value = value;
    }

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
    public override bool Equals(object? other) =>
        (other is NameId otherNameId
        && otherNameId.Value == Value)
        ||
        (other is string otherString
        && otherString == Value);

    /// <summary>
    /// Operator ==
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    public static bool operator ==(NameId n1, NameId n2) =>
        n1?.Value == n2?.Value;

    /// <summary>
    /// Operator !=
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    public static bool operator !=(NameId n1, NameId n2) =>
        n1?.Value != n2?.Value;

    /// <summary>
    /// Get hash code
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode() => 
        Value.GetHashCode();
}
