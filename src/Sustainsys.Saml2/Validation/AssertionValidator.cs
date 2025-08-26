using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.Validation;

/// <summary>
/// Saml Assertion validator
/// </summary>
public class AssertionValidator(TimeProvider timeProvider) : IAssertionValidator
{
    /// <inheritdoc/>
    public void Validate(
        Assertion assertion,
        AssertionValidationParameters parameters)
    {
        ValidateIssuer(assertion, parameters);
        ValidateConditions(assertion.Conditions, parameters);
        // Validate TrustLevel
        // Validate Conditions
        // Attributes: Attributes within an AttributeStatement must have at least one AttributeValue
    }

    /// <summary>
    /// Validate the issuer
    /// </summary>
    /// <param name="assertion">Saml assertion</param>
    /// <param name="parameters">Validation parameters</param>
    /// <exception cref="SamlValidationException">On validation failure</exception>
    protected virtual void ValidateIssuer(
        Assertion assertion,
        AssertionValidationParameters parameters)
    {
        if (assertion.Issuer != null &&
            assertion.Issuer != parameters.ValidIssuer)
        {
            throw new SamlValidationException(
                $"Assertion issuer {assertion.Issuer} does not match expected {parameters.ValidIssuer}");
        }
    }

    /// <summary>
    /// Validate conditions of an assertion
    /// </summary>
    /// <param name="conditions">Saml conditions</param>
    /// <param name="parameters">Validation parameters</param>
    /// <exception cref="SamlValidationException">On validation failure</exception>
    protected virtual void ValidateConditions(
        Conditions? conditions,
        AssertionValidationParameters parameters)
    {
        // TODO: Validate that conditions are not null - if they are throw SamlValidationException
        // TODO: Add Clock Skew Support

        if (conditions == null)
        {
            throw new SamlValidationException(
                $"No conditions found on assertion, conditions are required.");
        }

        // Core 2.5.1.2 NotBefore, NotOnOrAfter
        if (timeProvider.GetUtcNow() < conditions.NotBefore)
        {
            throw new SamlValidationException(
                        $"NotBefore {conditions.NotBefore} is after {timeProvider.GetUtcNow()}");
        }

        if (timeProvider.GetUtcNow() >= conditions.NotOnOrAfter)
        {
            throw new SamlValidationException(
                        $"NotOnOrAfter {conditions.NotOnOrAfter} is before {timeProvider.GetUtcNow()}");
        }

        // Core 2.5.1.4 AudienceRestriction, Audience
        foreach (var audienceRestriction in conditions.AudienceRestrictions)
        {
            if (!audienceRestriction.Audiences
                .Where(a => a != null)
                .Any(a => a == parameters.ValidAudience))
            {
                throw new SamlValidationException(
                    $"None of audiences {string.Join(", ", audienceRestriction.Audiences)} matches expected {parameters.ValidAudience}");
            }

        }

        // Core 2.5.1.6 ProxyRestriction. For first cut: throw NotImplementedException if there are any ProxyRestrictions
    }
}