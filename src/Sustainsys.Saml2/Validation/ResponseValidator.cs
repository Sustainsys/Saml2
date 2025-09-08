using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Samlp;

namespace Sustainsys.Saml2.Validation;

/// <summary>
/// Validates a Saml Response
/// </summary>
public class ResponseValidator(IValidator<Assertion, AssertionValidationParameters> assertionValidator)
    : IValidator<Response, ResponseValidationParameters>
{
    /// <inheritdoc/>
    public void Validate(
        Response samlResponse,
        ResponseValidationParameters validationParameters)
    {
        // Core 2.7.2 AuthnStatement
        ValidateVersion(samlResponse);
        // Core 3.2.1
        ValidateDestination(samlResponse, validationParameters);
        // Profile 4.1.4.2, 4.1.4.3
        ValidateIssuer(samlResponse, validationParameters);
        ValidateInResponseTo(samlResponse, validationParameters);
        ValidateStatusCode(samlResponse);

        ValidateAssertions(samlResponse.Assertions, validationParameters.AssertionValidationParameters);
    }

    /// <summary> 
    /// Validate the destination.
    /// </summary>
    /// <param name="samlResponse">Saml response</param>
    /// <param name="validationParameters">Validation parameters</param>
    /// <exception cref="ValidationException{Response}">On validation failure</exception>
    protected virtual void ValidateDestination(
     Response samlResponse,
     ResponseValidationParameters validationParameters)
    {
        if (samlResponse.Destination != null &&
            samlResponse.Destination != validationParameters.ValidDestination)
        {
            throw new ValidationException<Response>(
                $"Response destination {samlResponse.Destination} does not match expected {validationParameters.ValidDestination}");
        }
    }

    /// <summary>
    /// Validate InResponseTo
    /// </summary>
    /// <param name="samlResponse">Saml Response</param>
    /// <param name="validationParameters">Validation Parameters</param>
    /// <exception cref="ValidationException{Response}">On validation failure</exception>
    protected virtual void ValidateInResponseTo(Response samlResponse, ResponseValidationParameters validationParameters)
    {
        if (samlResponse.InResponseTo == null)
        {
            throw new ValidationException<Response>(
                $"InResponseTo is missing in response, expected {validationParameters.ValidInResponseTo}");
        }

        if (samlResponse.InResponseTo != validationParameters.ValidInResponseTo)
        {
            throw new ValidationException<Response>(
                $"InResponseTo {samlResponse.InResponseTo} doesn't match expected value {validationParameters.ValidInResponseTo}");
        }
    }

    /// <summary>
    /// Validate that the status code is <see cref="Constants.StatusCodes.Success"/>
    /// </summary>
    /// <param name="samlResponse">Saml Response</param>
    /// <exception cref="ValidationException{Response}">On validation failure</exception>
    protected virtual void ValidateStatusCode(Response samlResponse)
    {
        if (samlResponse.Status?.StatusCode?.Value != Constants.StatusCodes.Success)
        {
            throw new ValidationException<Response>($"Saml status code {samlResponse.Status?.StatusCode?.Value} is not success");
        }
    }

    /// <summary>
    /// Validate the issuer
    /// </summary>
    /// <param name="samlResponse">Saml response</param>
    /// <param name="validationParameters">Validation parameters</param>
    /// <exception cref="ValidationException{Response}">On validation failure</exception>
    protected virtual void ValidateIssuer(
        Response samlResponse,
        ResponseValidationParameters validationParameters)
    {
        if (samlResponse.Issuer != null)
        {
            if (samlResponse.Issuer.Value != validationParameters.ValidIssuer)
            {
                throw new ValidationException<Response>(
                    $"Response issuer {samlResponse.Issuer} does not match expected {validationParameters.ValidIssuer}");
            }

            if (samlResponse.Issuer.Format != null && samlResponse.Issuer.Format != Constants.NameIdFormats.Entity)
            {
                throw new ValidationException<Response>(
                    $"Issuer format {samlResponse.Issuer.Format} does not match {Constants.NameIdFormats.Entity} and must be null");
            }
        }
    }

    /// <summary>
    /// Validate the version
    /// </summary>
    /// <param name="samlResponse">Saml response</param>
    /// <exception cref="ValidationException{Response}">On validation failure</exception>
    protected virtual void ValidateVersion(Response samlResponse)
    {
        if (samlResponse.Version != "2.0")
        {
            throw new ValidationException<Response>($"Saml version \"{samlResponse.Version}\" is incorrect, it must be exactly \"2.0\"");
        }
    }

    /// <summary>
    /// Validate assertions.
    /// </summary>
    /// <param name="assertions">Assertions to validate</param>
    /// <param name="validationParameters">Validation Parameters</param>
    protected virtual void ValidateAssertions(
        IEnumerable<Assertion> assertions,
        AssertionValidationParameters validationParameters)
    {
        foreach (var assertion in assertions)
        {
            assertionValidator.Validate(assertion, validationParameters);
        }
    }
}