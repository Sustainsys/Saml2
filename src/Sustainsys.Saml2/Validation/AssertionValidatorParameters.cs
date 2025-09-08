using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.Validation;

/// <summary>
/// DTO carrying parameters for Saml assertion validation
/// </summary>
public class AssertionValidationParameters
{
    /// <summary>
    /// Valid issuer of and assertion
    /// </summary>
    /// <remarks>
    /// Deliberately only a plain string here as the issuer value and not
    /// a full <see cref="NameId"/> as there are special rules for the format
    /// when a <see cref="NameId"/> is used as an Issuer.
    /// </remarks>
    public string? ValidIssuer { get; set; }

    /// <summary>
    ///  A URI reference that identifies an intended audience.
    /// </summary>
    public string? ValidAudience { get; set; }

    /// <summary>
    /// A URI reference that identifies a protocol or mechanism to be used to confirm the subject.
    /// </summary>
    public string ValidSubjectConfirmationMethod { get; set; } = "urn:oasis:names:tc:SAML:2.0:cm:bearer";

    /// <summary>
    /// Valid Recipient that matches the assertion consumer service URL.
    /// </summary>
    public string? ValidRecipient { get; set; }

    /// <summary>
    /// The expected value for InResponseTo, set to null to allow unsolicited.
    /// </summary>
    public string? ValidInResponseTo { get; set; }
}