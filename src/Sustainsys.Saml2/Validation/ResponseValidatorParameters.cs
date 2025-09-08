using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.Validation;


/// <summary>
/// DTO carrying parameters for Saml response validation.
/// </summary>
public class ResponseValidationParameters
{
    /// <summary>
    /// Validation parameters for assertions embedded in the response.
    /// </summary>
    public required AssertionValidationParameters AssertionValidationParameters { get; set; }

    /// <summary>
    /// Valid issuer of the response and assertions - returns the ValidIssuer
    /// of the embedded SamlAssertionValidationParameters to ensure they are the same.
    /// </summary>
    public NameId? ValidIssuer { get => AssertionValidationParameters.ValidIssuer!; }

    /// <summary>
    /// Valid destination of the response and assertions. 
    /// A URI reference indicating the address to which this request has been sent.
    /// </summary>
    public string? ValidDestination { get => AssertionValidationParameters.ValidRecipient; }
}