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
        AssertionValidationParameters parameters)
    {
        // Validate Issuer
        // Validate TrustLevel
        // Validate Conditions
        // ...
    }

    /// <summary>
    /// Validate conditions of an assertion
    /// </summary>
    /// <param name="assertion">Saml assertion</param>
    /// <param name="validationParameters">Validation parameters</param>
    /// <exception cref="SamlValidationException">On validation failure</exception>
    protected virtual void ValidateConditions(
        Assertion assertion,
        AssertionValidationParameters validationParameters)
    {
        // Core 2.5.1.2 NotBefore, NotOnOrAfter
        // Core 2.5.1.4 AudienceRestriction, Audience
        // Core 2.5.1.5 OneTimeUse
        // Core 2.5.1.6 ProxyRestriction
    }
}