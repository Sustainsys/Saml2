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
        ValidateSubjectConfirmation(assertion, parameters);
        ValidateSubjectConfirmationData(assertion, parameters);
        // TODO: Validate Subject/SubjectConfirmation

        // TODO: Validate SubjectConfirmationData (Recipient, NotOnOrAfter, InResponseTo, Address)
        // Validate TrustLevel
        // Attributes: An AttributeStatement must have at least one Attribute (Core 2.7.3).
        // In our object model, we allow an empty list. The validation of empty AttributeStatements
        // is handled in the serializer.
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

        // Later: Core 2.5.1.6 ProxyRestriction. A simple service provider (client application) do not have
        // to care about proxy restrictions. They are only valid if the SP is a proxy that acts as an IDP
        // to other applications. We'll just ignore proxy restrictions for now.
    }
    /// <summary>
    /// Validate Subject Confirmation of an assertion. 
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="parameters"></param>
    /// <exception cref="SamlValidationException"></exception>
    protected virtual void ValidateSubjectConfirmation(Assertion assertion, AssertionValidationParameters parameters)
    {
        var method = assertion.Subject.SubjectConfirmation!.Method;

        if (method != null && method != parameters.ValidSubjectConfirmationMethod)
        {
            throw new SamlValidationException($"The method {method} in subject confirmation does not match the expected {parameters.ValidSubjectConfirmationMethod}");
        }
    }
    /// <summary>
    /// Validate Subject Confirmation Data .
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="parameters"></param>
    /// <exception cref="SamlValidationException"></exception>

    protected virtual void ValidateSubjectConfirmationData(Assertion assertion, AssertionValidationParameters parameters)
    {
        if (assertion.Subject.SubjectConfirmation!.SubjectConfirmationData == null)
        {
            throw new SamlValidationException("Required SubjectConfirmationData is missing in SubjectConfirmation");
        }
        var subjectConfirmationData = assertion.Subject.SubjectConfirmation!.SubjectConfirmationData!;
        var date = assertion.Subject.SubjectConfirmation!.SubjectConfirmationData!.NotOnOrAfter;
        var errors = new List<string>();

        if (subjectConfirmationData.Recipient != parameters.ValidRecipient)
        {
            errors.Add($"The recipient {subjectConfirmationData.Recipient} in subject confirmation data does not match the expected {parameters.ValidRecipient}.");
        }

        if (timeProvider.GetUtcNow() >= date)
        {
            errors.Add($"NotOnOrAfter {date} is before or equal to current time {timeProvider.GetUtcNow()}");
        }

        if (subjectConfirmationData.InResponseTo != parameters.ValidInResponseTo)
        {
            errors.Add($"The InResponseTo {subjectConfirmationData.InResponseTo} does not match the expected {parameters.ValidInResponseTo}");
        }

        if (errors.Any())
        {
            throw new SamlValidationException("SubjectConfirmationData validation is incorrect:\n" + string.Join("\n", errors));
        }
    }
}


