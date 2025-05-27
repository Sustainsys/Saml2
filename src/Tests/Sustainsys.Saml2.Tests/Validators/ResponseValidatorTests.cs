using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Validation;

namespace Sustainsys.Saml2.Tests.Validators;

public class ResponseValidatorTests
{
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
        var subject = new ResponseValidator();

        var response = CreateResponse();
        var parameters = CreateValidationParameters();

        // Should not throw.
        subject.Validate(response, parameters);
    }

    [Fact]
    public void Validate_Issuer_IsMissing()
    {
        var subject = new ResponseValidator();

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
        var subject = new ResponseValidator();

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
                .Should().Throw<SamlValidationException>()
                .WithMessage("*version*incorrect*");
        }
    }

    [Fact]
    public void Validate_Issuer_IsIncorrect()
    {
        var subject = new ResponseValidator();

        var response = CreateResponse();
        response.Issuer!.Value = "https://unexpected";

        var parameters = CreateValidationParameters();

        subject.Invoking(s => s.Validate(response, parameters))
            .Should().Throw<SamlValidationException>()
            .WithMessage("*issuer*https://unexpected*https://idp.example.com/Saml2*");

        // TODO: Validate NameID format once it is supported.
    }

    [Fact]
    public void Validate_Status_IsNonSuccess()
    {
        var subject = new ResponseValidator();

        var response = CreateResponse();
        response.Status.StatusCode.Value = Constants.StatusCodes.Requester;

        var parameters = CreateValidationParameters();

        subject.Invoking(s => s.Validate(response, parameters))
            .Should().Throw<SamlValidationException>()
            .WithMessage("*status*Requester*");
    }

    [Fact]
    public void Validate_Destination_IsIncorrect()
    {
        var subject = new ResponseValidator();

        var response = CreateResponse();
        response.Destination = "https://example.com/Acs";

        var parameters = CreateValidationParameters();
        parameters.ValidDestination = "https://example.com/AnotherEndpoint";

        subject.Invoking(s => s.Validate(response, parameters))
            .Should().Throw<SamlValidationException>()
            .WithMessage("*destination*https://example.com/Acs*");
    }

    [Fact]
    public void Validate_Destination_IsCorrect()
    {
        var subject = new ResponseValidator();

        var response = CreateResponse();
        response.Destination = "https://example.com/Acs";

        var parameters = CreateValidationParameters();
        parameters.ValidDestination = "https://example.com/Acs";

        subject.Invoking(s => s.Validate(response, parameters))
            .Should().NotThrow();
    }
}