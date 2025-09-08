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
            Issuer = "https://idp.example.com/Saml2",
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
                ValidIssuer = "https://idp.example.com/Saml2",
                ValidRecipient = "https://example.com/Acs"
            }
        };

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

    public static TheoryData<Action<Response>> Validate_IsSpecifiedOrIsMissing_Data =>
        new TheoryData<Action<Response>>
        {   
            // The happy path that should just validate the default response
            // from the factory with the default parameters from the factory.
            { r => { }},

            { r => {r.Issuer = null; }},
            { r => {r.Issuer!.Format = "urn:oasis:names:tc:SAML:2.0:nameid-format:entity"; }},
            { r => {r.Destination = "https://example.com/Acs"; }}
        };

    [Theory]
    [MemberData(nameof(Validate_IsSpecifiedOrIsMissing_Data))]
    public void Validate_IsSpecifiedOrIsMissing(Action<Response> modify)
    {
        var subject = CreateSubject();
        var response = CreateResponse();

        modify(response);

        var parameters = CreateValidationParameters();

        subject.Invoking(s => s.Validate(response, parameters))
            .Should().NotThrow();
    }

    public static TheoryData<Action<Response>, string> Validate_IsMissingOrIncorrect_Data =>
        new TheoryData<Action<Response>, string>
        {
            { r => { r.Issuer = "https://unexpected"; }, "*issuer*https://unexpected*https://idp.example.com/Saml2*" },
            { r => { r.Issuer!.Format = "urn:Invalid"; }, "*format*urn:invalid*urn:oasis:names:tc:SAML:2.0:nameid-format:entity*" },
            { r => { r.Destination = "https://example.com/OtherAcs"; }, "*destination*https://example.com/OtherAcs*" },
            { r => { r.Status.StatusCode.Value = Constants.StatusCodes.Requester; }, "*status*Requester*" }
        };

    [Theory]
    [MemberData(nameof(Validate_IsMissingOrIncorrect_Data))]
    public void Validate_IsMissingOrIncorrect(Action<Response> destroy, string messagePattern)
    {
        var subject = CreateSubject();
        var response = CreateResponse();

        destroy(response);

        var parameters = CreateValidationParameters();

        subject.Invoking(s => s.Validate(response, parameters))
            .Should().Throw<ValidationException<Response>>()
            .WithMessage($"*{messagePattern}*");
    }
}