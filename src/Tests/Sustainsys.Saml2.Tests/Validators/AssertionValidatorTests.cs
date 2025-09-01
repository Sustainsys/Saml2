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
            Subject = new()
            {
                SubjectConfirmation = new()
                {
                    Method = "urn:oasis:names:tc:SAML:2.0:cm:bearer",
                    SubjectConfirmationData = new()
                    {
                        NotBefore = new(2025, 05, 28, 9, 34, 40),
                        NotOnOrAfter = new(2025, 05, 28, 9, 39, 40),
                        Recipient = "https://sp.example.com/Saml2/Acs",
                        InResponseTo = "b123456"
                    }
                }
            },
            Conditions = new()
            {
                // Check with current time from TimeProvider
                NotBefore = new(2025, 05, 28, 9, 34, 42),
                NotOnOrAfter = new(2026, 05, 28, 9, 34, 41),
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
            ValidAudience = "https://sp.example.com/Saml2",
            ValidRecipient = "https://sp.example.com/Saml2/Acs",
            ValidInResponseTo = "b123456"
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
    public void Validate_Conditions_IsNotNull()
    {
        var subject = CreateSubject();

        var assertion = CreateAssertion();
        assertion.Conditions = null;
        var parameters = CreateValidationParameters();

        subject.Invoking(subject => subject.Validate(assertion, parameters))
            .Should().Throw<SamlValidationException>()
            .WithMessage("*conditions*");
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
        assertion.Conditions!.NotBefore = new(2025, 05, 28, 9, 34, 43);
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
        assertion.Conditions!.NotOnOrAfter = new(2025, 05, 28, 9, 34, 42);
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
        assertion.Conditions!.AudienceRestrictions[0].Audiences[1] = "https://unexpected";

        var parameters = CreateValidationParameters();

        subject.Invoking(s => s.Validate(assertion, parameters))
            .Should().Throw<SamlValidationException>()
            .WithMessage($"*audiences*expected https://sp.example.com/Saml2");
    }

    public static TheoryData<Action<Assertion>, string> Validate_MissingOrIncorrect_Data =>
        new TheoryData<Action<Assertion>, string>
        {
            {a => {a.Subject = null!; },  "*subject*" },
            {a => {a.Subject!.SubjectConfirmation = null!; }, "*subjectconfirmation*"},
            {a => {a.Subject!.SubjectConfirmation!.Method = null!;}, "*method*subjectconfirmation*" },
            {a => {a.Subject!.SubjectConfirmation!.SubjectConfirmationData = null!;}, "*subjectconfirmationdata*missing*"},
            {a => {a.Subject!.SubjectConfirmation!.SubjectConfirmationData!.Recipient = null!; }, "*recipient  *required*"},
            {a => {a.Subject!.SubjectConfirmation!.SubjectConfirmationData!.NotOnOrAfter = null!; }, "*notonorafter*required*"},
            {a => {a.Subject!.SubjectConfirmation!.Method ="urn:Invalid"; }, $"*confirmation*urn:oasis:names:tc:SAML:2.0:cm:bearer*"},
            {a => {
                a.Subject!.SubjectConfirmation!.SubjectConfirmationData!.NotOnOrAfter = new(2024, 02, 10, 17, 50, 13);
                a.Subject.SubjectConfirmation.SubjectConfirmationData.InResponseTo = "123";
                a.Subject.SubjectConfirmation.SubjectConfirmationData.Recipient = "https://invalid";
            },"*SubjectConfirmationData*incorrect*"},
            {a => {a.Subject!.SubjectConfirmation!.SubjectConfirmationData!.Recipient ="https://unexpected"; },"*recipient*https://unexpected*https://sp.example.com/Saml2/Acs*" },
            {a => {a.Subject!.SubjectConfirmation!.SubjectConfirmationData!.NotBefore = new(2025, 05, 28, 9, 34, 43); },"*notbefore*" },
            {a => {a.Subject!.SubjectConfirmation!.SubjectConfirmationData!.NotOnOrAfter = new(2024,02,10,17,50,13); }, "*notonorafter*" },
            {a => {a.Subject!.SubjectConfirmation!.SubjectConfirmationData!.InResponseTo = "1234"; }, "*inresponseto*b123456*" },
        };

    [Theory]
    [MemberData(nameof(Validate_MissingOrIncorrect_Data))]
    public void Validate_MissingOrIncorrect(Action<Assertion> destroy, string messagePattern)
    {
        var subject = CreateSubject();
        var assertion = CreateAssertion();

        destroy(assertion);

        var parameters = CreateValidationParameters();

        subject.Invoking(s => s.Validate(assertion, parameters))
            .Should().Throw<SamlValidationException>()
            .WithMessage(messagePattern);
    }
}