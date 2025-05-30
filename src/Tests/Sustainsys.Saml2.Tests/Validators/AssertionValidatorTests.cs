using Microsoft.Extensions.Time.Testing;
using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Validation;

namespace Sustainsys.Saml2.Tests.Validators;

public class AssertionValidatorTests
{
    private static AssertionValidator CreateSubject() =>
        new(new FakeTimeProvider(new(2025, 05, 28, 9, 34, 42, TimeSpan.Zero)));

    private static Assertion CreateAssertion() =>
        new()
        {
            Issuer = new()
            {
                // Should match value from validation parameters.
                Value = "https://idp.example.com/Saml2"
            },
            Conditions = new()
            {
                // Check with current time from TimeProvider
                // TODO: Set sample values that match the Fake above.
                NotBefore = new(2025, 05, 28, 9, 34, 42, DateTimeKind.Utc),
                NotOnOrAfter = new(2026, 05, 28, 9, 34, 41, DateTimeKind.Utc),
                AudienceRestrictions =
                {
                    new()
                    {
                        Audiences =
                        {
                            // At least one value should match validation params.
                            "https://other.value.example.com/Saml1.1",
                            "https://sp.example.com/Saml2",
                        }
                    }
                }
            },
        };

    private static AssertionValidationParameters CreateValidationParameters() =>
        new()
        {
            ValidIssuer = new()
            {
                Value = "https://idp.example.com/Saml2"
            },
            // TODO: One expected audience. The assertion should contain this, but
            // the assertion may contain other values too - that's fine.
            ValidAudience = "https://sp.example.com/Saml2",
        };

    // The happy path that should just validate the default response
    // from the factory with the default parameters from the factory.
    [Fact]
    public void Validate()
    {
        var subject = CreateSubject();

        var assertion = CreateAssertion();
        var parameters = CreateValidationParameters();

        // Should not throw.
        subject.Validate(assertion, parameters);
    }

    [Fact]
    public void Validate_Issuer_IsIncorrect()
    {
        var subject = CreateSubject();

        var assertion = CreateAssertion();
        assertion.Issuer!.Value = "https://unexpected";

        var parameters = CreateValidationParameters();

        subject.Invoking(s => s.Validate(assertion, parameters))
            .Should().Throw<SamlValidationException>()
            .WithMessage("*issuer*https://unexpected*https://idp.example.com/Saml2*");
    }

    [Fact]
    public void Validate_Conditions_AudienceRestriction_IsMissing()
    {
        // AudienceRestriction is indeed optional, so if it is missing, validation should pass.
        var subject = CreateSubject();

        var assertion = CreateAssertion();
        assertion.Conditions!.AudienceRestrictions.Clear();
        var parameters = CreateValidationParameters();

        subject.Validate(assertion, parameters);
    }

    [Fact]
    public void Validate_Conditions_NotBefore_IsMissing()
    {
        // NotBefore is indeed optional, so if it is missing, validation should pass.
        var subject = CreateSubject();

        var assertion = CreateAssertion();
        assertion.Conditions!.NotBefore = null;
        var parameters = CreateValidationParameters();

        // Should not throw.
        subject.Validate(assertion, parameters);

    }

    [Fact]
    public void Validate_Conditions_NotOnOrAfter_IsMissing()
    {
        var subject = CreateSubject();

        var assertion = CreateAssertion();
        assertion.Conditions!.NotOnOrAfter = null;
        var parameters = CreateValidationParameters();
        // Should not throw.
        subject.Validate(assertion, parameters);
    }

    [Fact]
    public void Validate_Conditions_NotBefore_IsBefore()
    {
        var subject = CreateSubject();

        var assertion = CreateAssertion();
        assertion.Conditions!.NotBefore = new(2025, 05, 28, 9, 34, 43, DateTimeKind.Utc);
        var parameters = CreateValidationParameters();

        subject.Invoking(s => s.Validate(assertion, parameters))
           .Should().Throw<SamlValidationException>()
           .WithMessage("*notbefore*");
    }
    [Fact]
    public void Validate_Conditions_NotOnOrAfter_IsAfter()
    {
        var subject = CreateSubject();

        var assertion = CreateAssertion();
        assertion.Conditions!.NotOnOrAfter = new(2025, 05, 28, 9, 34, 42, DateTimeKind.Utc);
        var parameters = CreateValidationParameters();

        subject.Invoking(s => s.Validate(assertion, parameters))
            .Should().Throw<SamlValidationException>()
            .WithMessage("*notonorafter*");
    }

    [Fact]
    public void Validate_Conditions_AudienceRestrictions_IsInCorrect()
    {
        var subject = CreateSubject();

        var assertion = CreateAssertion();
        foreach (var audienceRestriction in assertion.Conditions!.AudienceRestrictions)
        {

            for (int i = 0; i < audienceRestriction.Audiences.Count; i++)
            {
                audienceRestriction.Audiences[i] = "https://unexpected";
            }
        }

        var parameters = CreateValidationParameters();

        subject.Invoking(s => s.Validate(assertion, parameters))
    .Should().Throw<SamlValidationException>()
    .WithMessage($"Assertion conditions do not match expected https://sp.example.com/Saml2");

    }
}