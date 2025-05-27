using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Validation;

namespace Sustainsys.Saml2.Tests.Validators;

public class AssertionValidatorTests
{
    private static Assertion CreateAssertion() =>
        new()
        {
            Issuer = new()
            {
                Value = "https://idp.example.com/Saml2"
            },
        };

    private static AssertionValidationParameters CreateValidationParameters() =>
        new()
        {
            ValidIssuer = new()
            {
                Value = "https://idp.example.com/Saml2"
            }
        };

    // The happy path that should just validate the default response
    // from the factory with the default parameters from the factory.
    [Fact]
    public void Validate()
    {
        var subject = new AssertionValidator();

        var assertion = CreateAssertion();
        var parameters = CreateValidationParameters();

        // Should not throw.
        subject.Validate(assertion, parameters);
    }
}
