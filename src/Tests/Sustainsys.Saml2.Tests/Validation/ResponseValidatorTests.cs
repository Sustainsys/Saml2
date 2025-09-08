using Microsoft.Extensions.Time.Testing;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Validation;

namespace Sustainsys.Saml2.Tests.Validators;

public class ResponseValidatorTests
{
    private static ResponseValidator CreateSubject() =>
     new(
         new AssertionValidator(
             new FakeTimeProvider(new(2025, 05, 28, 11, 14, 53, TimeSpan.Zero))));

    private static Response CreateResponse() =>
        new()
        {
            Issuer = new()
            {
                Value = "https://idp.example.com/Saml2"
            },
            Status = new()
            {
                StatusCode = new()
                {
                    Value = Constants.StatusCodes.Success
                }
            }
        };

    private static ResponseValidationParameters CreateValidationParameters() =>
        new()
        {
            AssertionValidationParameters = new()
            {
                ValidIssuer = "https://idp.example.com/Saml2"
            }
        };

    // The happy path that should just validate the default response
    // from the factory with the default parameters from the factory.
    [Fact]
    public void Validate()
    {
        var subject = CreateSubject();

        var response = CreateResponse();
        var parameters = CreateValidationParameters();

        // Should not throw.
        subject.Validate(response, parameters);
    }

    [Fact]
    public void Validate_Issuer_IsMissing()
    {
        var subject = CreateSubject();

        var response = CreateResponse();
        response.Issuer = null;

        var parameters = CreateValidationParameters();

        // Should not throw. Missing issuers are allowed.
        subject.Validate(response, parameters);
    }

    [Theory]
    [InlineData("2.0", true)]
    [InlineData("2.1", false)]
    [InlineData(null, false)]
    public void Validate_Version(string? version, bool valid)
    {
        var subject = CreateSubject();

        var response = CreateResponse();

        // Yes, it can be null - we're testing!
        response.Version = version!;

        var parameters = CreateValidationParameters();

        if (valid)
        {
            subject.Validate(response, parameters);
        }
        else
        {
            subject.Invoking(s => s.Validate(response, parameters))
                .Should().Throw<ValidationException<Response>>()
                .WithMessage("*version*incorrect*");
        }
    }

    [Fact]
    public void Validate_Issuer_IsIncorrect()
    {
        var subject = CreateSubject();

        var response = CreateResponse();
        response.Issuer!.Value = "https://unexpected";

        var parameters = CreateValidationParameters();

        subject.Invoking(s => s.Validate(response, parameters))
            .Should().Throw<ValidationException<Response>>()
            .WithMessage("*issuer*https://unexpected*https://idp.example.com/Saml2*");
    }

    [Fact]
    public void Validate_Issuer_FormatIsIncorrect()
    {
        var subject = CreateSubject();
        var response = CreateResponse();
        response.Issuer!.Format = "urn:invalid";
        var parameters = CreateValidationParameters();
        subject.Invoking(s => s.Validate(response, parameters))
            .Should().Throw<ValidationException<Response>>()
            .WithMessage("*format*urn:invalid*urn:oasis:names:tc:SAML:2.0:nameid-format:entity*");
    }

    [Fact]
    public void Validate_Issuer_FormatIsSpecified()
    {
        var subject = CreateSubject();
        var response = CreateResponse();
        response.Issuer!.Format = "urn:oasis:names:tc:SAML:2.0:nameid-format:entity";
        var parameters = CreateValidationParameters();

        // Should not Throw
        subject.Validate(response, parameters);
    }

    [Fact]
    public void Validate_Status_IsNonSuccess()
    {
        var subject = CreateSubject();

        var response = CreateResponse();
        response.Status.StatusCode.Value = Constants.StatusCodes.Requester;

        var parameters = CreateValidationParameters();

        subject.Invoking(s => s.Validate(response, parameters))
            .Should().Throw<ValidationException<Response>>()
            .WithMessage("*status*Requester*");
    }

    [Fact]
    public void Validate_Destination_IsIncorrect()
    {
        var subject = CreateSubject();

        var response = CreateResponse();
        response.Destination = "https://example.com/Acs";

        var parameters = CreateValidationParameters();
        parameters.AssertionValidationParameters.ValidRecipient = "https://example.com/AnotherEndpoint";

        subject.Invoking(s => s.Validate(response, parameters))
            .Should().Throw<ValidationException<Response>>()
            .WithMessage("*destination*https://example.com/Acs*");
    }

    [Fact]
    public void Validate_Destination_IsCorrect()
    {
        var subject = CreateSubject();

        var response = CreateResponse();
        response.Destination = "https://example.com/Acs";

        var parameters = CreateValidationParameters();
        parameters.AssertionValidationParameters.ValidRecipient = "https://example.com/Acs";

        subject.Invoking(s => s.Validate(response, parameters))
            .Should().NotThrow();
    }
}