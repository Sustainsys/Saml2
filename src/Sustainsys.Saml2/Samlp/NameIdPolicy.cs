namespace Sustainsys.Saml2.Samlp;

/// <summary>
/// Element NameIDPolicy, Core 3.4.1.1
/// </summary>
public class NameIdPolicy
{
    /// <summary>
    /// An URI reference. 
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Specifies in which namespace the identifier should be returned.
    /// </summary>
    public string? SPNameQualifier { get; set; }

    /// <summary>
    /// If the requester grants to the Identity provider.
    /// Default is false.
    /// </summary>
    public bool? AllowCreate { get; set; } = false;
}