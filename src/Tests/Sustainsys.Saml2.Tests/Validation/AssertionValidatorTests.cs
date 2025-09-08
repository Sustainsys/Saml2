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
                Format = "urn:oasis:names:tc:SAML:2.0:nameid-format:entity",
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
            ValidIssuer = "https://idp.example.com/Saml2",
            ValidAudience = "https://sp.example.com/Saml2",
            ValidRecipient = "https://sp.example.com/Saml2/Acs",
            ValidInResponseTo = "b123456"
        };

    public static TheoryData<Action<Assertion>> Validate_MissingIsValid_Data =>
        new TheoryData<Action<Assertion>>
        {
            // The happy path that should just validate the default response
            // from the factory with the default parameters from the factory.
            {a => { } },

            // Removing non-required things should still pass validation.
            {a => {a.Conditions!.NotOnOrAfter = null!;}},
            {a => {a.Conditions!.NotBefore = null!;}},
            {a => {a.Conditions!.AudienceRestrictions.Clear();}},

            // Removing Issuer format is ok
            {a => {a.Issuer!.Format = null!;}}
        };

    [Theory]
    [MemberData(nameof(Validate_MissingIsValid_Data))]
    public void Validate_MissingIsValid(Action<Assertion> destroy)
    {
        var subject = CreateSubject();
        var assertion = CreateAssertion();

        destroy(assertion);

        var parameters = CreateValidationParameters();

        subject.Invoking(s => s.Validate(assertion, parameters))
            .Should().NotThrow();
    }

    public static TheoryData<Action<Assertion>, string> Validate_MissingOrIncorrect_Data =>
        new TheoryData<Action<Assertion>, string>
        {
            {a => {a.Subject = null!;}, "*subject*"},
            {a => {a.Subject!.SubjectConfirmation = null!;}, "*subjectconfirmation*"},
            {a => {a.Subject!.SubjectConfirmation!.Method = null!;}, "*method*subjectconfirmation*"},
            {a => {a.Subject!.SubjectConfirmation!.SubjectConfirmationData = null!;}, "*subjectconfirmationdata*missing*"},
            {a => {a.Subject!.SubjectConfirmation!.SubjectConfirmationData!.Recipient = null!;}, "*recipient  *required*"},
            {a => {a.Subject!.SubjectConfirmation!.SubjectConfirmationData!.NotOnOrAfter = null!;}, "*notonorafter*required*"},
            {a => {a.Subject!.SubjectConfirmation!.Method ="urn:Invalid"; }, $"*confirmation*urn:oasis:names:tc:SAML:2.0:cm:bearer*"},
            {a => {
                a.Subject!.SubjectConfirmation!.SubjectConfirmationData!.NotOnOrAfter = new(2024, 02, 10, 17, 50, 13);
                a.Subject.SubjectConfirmation.SubjectConfirmationData.InResponseTo = "123";
                a.Subject.SubjectConfirmation.SubjectConfirmationData.Recipient = "https://invalid";
            },"*SubjectConfirmationData*incorrect*Recipient*NotOnOrAfter*InResponseTo*"},
            {a => {a.Subject!.SubjectConfirmation!.SubjectConfirmationData!.Recipient ="https://unexpected";},"*recipient*https://unexpected*https://sp.example.com/Saml2/Acs*"},
            {a => {a.Subject!.SubjectConfirmation!.SubjectConfirmationData!.NotBefore = new(2025, 05, 28, 9, 34, 43);},"*notbefore*"},
            {a => {a.Subject!.SubjectConfirmation!.SubjectConfirmationData!.NotOnOrAfter = new(2024,02,10,17,50,13);}, "*notonorafter*"},
            {a => {a.Subject!.SubjectConfirmation!.SubjectConfirmationData!.InResponseTo = "1234";}, "*inresponseto*b123456*"},
            {a => {a.Conditions!.AudienceRestrictions[0].Audiences[1] = "https://unexpected";}, "*audiences*expected https://sp.example.com/Saml2*"},
            {a => {a.Conditions!.NotOnOrAfter = new(2025, 05, 28, 9, 34, 42);}, "*notonorafter*"},
            {a => {a.Conditions!.NotBefore = new(2025, 05, 28, 9, 34, 43);}, "*notbefore*"},
            {a => {a.Conditions = null!;}, "*conditions*"},
            {a => {a.Issuer = null!;}, "*issuer*missing*"},
            {a => {a.Issuer.Value = "https://unexpected";},"*issuer*https://unexpected*https://idp.example.com/Saml2*"},
            {a => {a.Issuer.Format ="urn:invalid"; },"*issuer*format*urn:oasis:names:tc:SAML:2.0:nameid-format:entity*"},
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
            .Should().Throw<ValidationException<Assertion>>()
            .WithMessage(messagePattern);
    }
}