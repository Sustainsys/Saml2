namespace Sustainsys.Saml2.Samlp;

/// <summary>
/// Element NameIDPolicy, Core 3.4.1.1
/// </summary>
public class NameIdPolicy
{
    /// <summary>
    /// A URI reference identifying the name identifier format.
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Specifies in which namespace the identifier should be returned.
    /// </summary>
    public string? SPNameQualifier { get; set; }

    /// <summary>
    /// If the requester grants permission to the Identity provider to
    /// create a new identifier. If no value is present it indicates false.
    /// </summary>
    public bool? AllowCreate { get; set; }
}