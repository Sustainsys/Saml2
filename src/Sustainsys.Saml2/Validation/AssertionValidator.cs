using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.Validation;

/// <summary>
/// Saml Assertion validator
/// </summary>
public class AssertionValidator : IAssertionValidator
{
    /// <inheritdoc/>
    public void Validate(
        Assertion assertion,
        SamlAssertionValidationParameters parameters)
    {
        // TODO: Remember to validate issuer.
    }
}