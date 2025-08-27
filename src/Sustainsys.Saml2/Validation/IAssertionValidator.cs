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
    void Validate(Assertion assertion, AssertionValidationParameters parameters);
}

/// <summary>
/// DTO carrying parameters for Saml assertion validation
/// </summary>
public class AssertionValidationParameters
{
    /// <summary>
    /// Valid issuer of the response and assertions
    /// </summary>
    public NameId? ValidIssuer { get; set; }

    /// <summary>
    ///  A URI reference that identifies an intended audience.
    /// </summary>
    public string? ValidAudience { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ValidSubjectConfirmationMethod { get; set; } = "urn:oasis:names:tc:SAML:2.0:cm:bearer";

    /// <summary>
    /// Valid Recipient that matches the assertion consumer service URL.
    /// </summary>
    public string? ValidRecipient { get; set; }
}