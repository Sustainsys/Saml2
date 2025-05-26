using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.Validation;

/// <summary>
/// Validates an asseriton
/// </summary>
public interface IAssertionValidator
{
    /// <summary>
    /// Validate a Saml assertion
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="parameters"></param>
    void Validate(Assertion assertion, SamlAssertionValidationParameters parameters);
}

/// <summary>
/// DTO carrying parameters for Saml assertion validation
/// </summary>
public class SamlAssertionValidationParameters
{
    /// <summary>
    /// Valid issuer of the response and assertions
    /// </summary>
    public NameId? ValidIssuer { get; set; }
}