using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Samlp;

namespace Sustainsys.Saml2.Validation;

/// <summary>
/// Validates a Saml response
/// </summary>
public interface ISamlResponseValidator
{
    /// <summary>
    /// Validates a Saml response.
    /// </summary>
    /// <param name="samlResponse"></param>
    /// <param name="parameters">Expected values and settings for validation</param>
    /// <exception cref="SamlValidationException">On validation failure</exception>
    void Validate(SamlResponse samlResponse, SamlResponseValidationParameters parameters);
}


/// <summary>
/// DTO carrying parameters for Saml response validation.
/// </summary>
public class SamlResponseValidationParameters
{
    /// <summary>
    /// Validation parameters for assertions embedded in the response.
    /// </summary>
    public required SamlAssertionValidationParameters AssertionValidationParameters { get; set; }

    /// <summary>
    /// Valid issuer of the response and assertions - returns the ValidIssuer
    /// of the embedded SamlAssertionValidationParameters to ensure they are the same.
    /// </summary>
    public NameId? ValidIssuer { get => AssertionValidationParameters.ValidIssuer; }
}