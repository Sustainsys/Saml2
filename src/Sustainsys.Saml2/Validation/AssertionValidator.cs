using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.Validation;

/// <summary>
/// Saml Assertion validator
/// </summary>
public class AssertionValidator(TimeProvider timeProvider) : IValidator<Assertion, AssertionValidationParameters>
{
    /// <inheritdoc/>
    public void Validate(
        Assertion assertion,
        AssertionValidationParameters parameters)
    {
        ValidateIssuer(assertion, parameters);
        ValidateConditions(assertion.Conditions, parameters);
        ValidateSubject(assertion.Subject, parameters);

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
    /// <exception cref="ValidationException{Assertion}">On validation failure</exception>
    protected virtual void ValidateIssuer(
        Assertion assertion,
        AssertionValidationParameters parameters)
    {
        if (assertion.Issuer == null)
        {
            throw new ValidationException<Assertion>("Issuer is missing");
        }

        if (assertion.Issuer.Value != parameters.ValidIssuer)
        {
            throw new ValidationException<Assertion>(
                $"Assertion issuer value {assertion.Issuer.Value} does not match expected {parameters.ValidIssuer}");
        }

        if (assertion.Issuer.Format != null && assertion.Issuer.Format != Constants.NameIdFormats.Entity)
        {
            throw new ValidationException<Assertion>(
                $"Issuer format does not match {Constants.NameIdFormats.Entity} and must be null");
        }
    }

    /// <summary>
    /// Validate conditions of an assertion
    /// </summary>
    /// <param name="conditions">Saml conditions</param>
    /// <param name="parameters">Validation parameters</param>
    /// <exception cref="ValidationException{Assertion}">On validation failure</exception>
    protected virtual void ValidateConditions(
        Conditions? conditions,
        AssertionValidationParameters parameters)
    {
        // TODO: Add Clock Skew Support

        if (conditions == null)
        {
            throw new ValidationException<Assertion>(
                $"No conditions found on assertion, conditions are required.");
        }

        // Core 2.5.1.2 NotBefore, NotOnOrAfter
        if (timeProvider.GetUtcNow() < conditions.NotBefore)
        {
            throw new ValidationException<Assertion>(
                        $"NotBefore {conditions.NotBefore} is after {timeProvider.GetUtcNow()}");
        }

        if (timeProvider.GetUtcNow() >= conditions.NotOnOrAfter)
        {
            throw new ValidationException<Assertion>(
                        $"NotOnOrAfter {conditions.NotOnOrAfter} is before {timeProvider.GetUtcNow()}");
        }

        // Core 2.5.1.4 AudienceRestriction, Audience
        foreach (var audienceRestriction in conditions.AudienceRestrictions)
        {
            if (!audienceRestriction.Audiences
                .Where(a => a != null)
                .Any(a => a == parameters.ValidAudience))
            {
                throw new ValidationException<Assertion>(
                    $"None of audiences {string.Join(", ", audienceRestriction.Audiences)} matches expected {parameters.ValidAudience}");
            }

        }

        // Later: Core 2.5.1.6 ProxyRestriction. A simple service provider (client application) do not have
        // to care about proxy restrictions. They are only valid if the SP is a proxy that acts as an IDP
        // to other applications. We'll just ignore proxy restrictions for now.
    }

    /// <summary>
    /// Validate Subject of an assertion.
    /// </summary>
    /// <param name="subject">Saml subject</param>
    /// <param name="parameters">Validation parameters</param>
    /// <exception cref="ValidationException{Assertion}">On validation failure</exception>
    protected virtual void ValidateSubject(Subject? subject, AssertionValidationParameters parameters)
    {
        if (subject == null)
        {
            throw new ValidationException<Assertion>($"No subject found on assertion, subject is required.");
        }
        ValidateSubjectConfirmation(subject.SubjectConfirmation, parameters);
    }

    /// <summary>
    /// Validate Subject Confirmation of an assertion. 
    /// </summary>
    /// <param name="subjectConfirmation">Saml subjectConfirmation</param>
    /// <param name="parameters">Validation parameters</param>
    /// <exception cref="ValidationException{Assertion}">On validation failure</exception>
    protected virtual void ValidateSubjectConfirmation(SubjectConfirmation? subjectConfirmation, AssertionValidationParameters parameters)
    {
        if (subjectConfirmation == null)
        {
            throw new ValidationException<Assertion>($"No SubjectConfirmation found on assertion, SubjectConfirmation is required.");
        }

        var method = subjectConfirmation.Method;

        if (method != parameters.ValidSubjectConfirmationMethod)
        {
            throw new ValidationException<Assertion>($"The method {method} in SubjectConfirmation does not match the expected {parameters.ValidSubjectConfirmationMethod}");
        }
        ValidateSubjectConfirmationData(subjectConfirmation.SubjectConfirmationData, parameters);
    }

    /// <summary>
    /// Validate Subject Confirmation Data.
    /// </summary>
    /// <param name="subjectConfirmationData">Saml subjectConfirmationData</param>
    /// <param name="parameters">Validation parameters</param>
    /// <exception cref="ValidationException{Assertion}">On validation failure</exception>
    protected virtual void ValidateSubjectConfirmationData(SubjectConfirmationData? subjectConfirmationData, AssertionValidationParameters parameters)
    {
        if (subjectConfirmationData == null)
        {
            throw new ValidationException<Assertion>($"SubjectConfirmationData is missing, SubjectConfirmationData is required.");
        }

        var notOnOrAfter = subjectConfirmationData.NotOnOrAfter;
        var notBefore = subjectConfirmationData.NotBefore;
        var errors = new List<string>();

        if (subjectConfirmationData.Recipient != parameters.ValidRecipient)
        {
            errors.Add($"The recipient {subjectConfirmationData.Recipient} in subject confirmation data does not match the expected {parameters.ValidRecipient}.");
        }

        if (subjectConfirmationData.Recipient == null)
        {
            errors.Add($"Recipient is required in SubjectConfirmationData");
        }

        if (timeProvider.GetUtcNow() >= notOnOrAfter)
        {
            errors.Add($"NotOnOrAfter {notOnOrAfter} has passed, time is now {timeProvider.GetUtcNow()}");
        }

        if (notOnOrAfter == null)
        {
            errors.Add($"NotOnOrAfter is required in SubjectConfirmationData");
        }

        if (notBefore.HasValue && timeProvider.GetUtcNow() < notBefore)
        {
            errors.Add($"NotBefore {notBefore} is after current time {timeProvider.GetUtcNow()}");
        }

        if (subjectConfirmationData.InResponseTo != parameters.ValidInResponseTo)
        {
            errors.Add($"The InResponseTo {subjectConfirmationData.InResponseTo} does not match the expected {parameters.ValidInResponseTo}");
        }

        if (errors.Any())
        {
            throw new ValidationException<Assertion>("SubjectConfirmationData validation is incorrect:\n" + string.Join("\n", errors));
        }
    }
}