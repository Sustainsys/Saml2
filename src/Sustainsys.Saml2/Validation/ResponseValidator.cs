using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Samlp;

namespace Sustainsys.Saml2.Validation;

/// <summary>
/// Validates a Saml Response
/// </summary>
public class ResponseValidator : IResponseValidator
{
    /// <inheritdoc/>
    public void Validate(
        Response samlResponse,
        SamlResponseValidationParameters validationParameters)
    {
        // TODO: ValidateDestination

        ValidateAssertions(samlResponse.Assertions, validationParameters.AssertionValidationParameters);
        // Core 2.7.2 AuthnStatement
        ValidateVersion(samlResponse);

        // Profile 4.1.4.2, 4.1.4.3
        ValidateIssuer(samlResponse, validationParameters);
        ValidateStatusCode(samlResponse);
    }

    /// <summary>
    /// Validate that the status code is <see cref="Constants.StatusCodes.Success"/>
    /// </summary>
    /// <param name="samlResponse">Saml Response</param>
    /// <exception cref="SamlValidationException">On validation failure</exception>
    protected virtual void ValidateStatusCode(Response samlResponse)
    {
        if (samlResponse.Status?.StatusCode?.Value != Constants.StatusCodes.Success)
        {
            throw new SamlValidationException($"Saml status code {samlResponse.Status?.StatusCode?.Value} is not success");
        }
    }

    /// <summary>
    /// Validate the issuer
    /// </summary>
    /// <param name="samlResponse">Saml response</param>
    /// <param name="validationParameters">Validation parameters</param>
    /// <exception cref="SamlValidationException">On validation failure</exception>
    protected virtual void ValidateIssuer(
        Response samlResponse,
        SamlResponseValidationParameters validationParameters)
    {
        if (samlResponse.Issuer != null &&
            samlResponse.Issuer != validationParameters.ValidIssuer)
        {
            throw new SamlValidationException(
                $"Response issuer {samlResponse.Issuer} does not match expected {validationParameters.ValidIssuer}");
        }
    }

    /// <summary>
    /// Validate the version
    /// </summary>
    /// <param name="samlResponse">Saml response</param>
    /// <exception cref="SamlValidationException">On validation failure</exception>
    protected virtual void ValidateVersion(Response samlResponse)
    {
        if (samlResponse.Version != "2.0")
        {
            throw new SamlValidationException($"Saml version \"{samlResponse.Version}\" is incorrect, it must be exactly \"2.0\"");
        }
    }

    /// <summary>
    /// Validate assertions.
    /// </summary>
    /// <param name="assertions">Assertions to validate</param>
    /// <param name="validationParameters">Validation Parameters</param>
    protected virtual void ValidateAssertions(
        IEnumerable<Assertion> assertions,
        SamlAssertionValidationParameters validationParameters)
    {
        foreach (var assertion in assertions)
        {
            // Core 2.5.1
            ValidateConditions(assertion, validationParameters);
        }
    }

    /// <summary>
    /// Validate conditions of an assertion
    /// </summary>
    /// <param name="assertion">Saml assertion</param>
    /// <param name="validationParameters">Validation parameters</param>
    /// <exception cref="SamlValidationException">On validation failure</exception>
    protected virtual void ValidateConditions(
        Assertion assertion,
        SamlAssertionValidationParameters validationParameters)
    {
        // Core 2.5.1.2 NotBefore, NotOnOrAfter
        // Core 2.5.1.4 AudienceRestriction, Audience
        // Core 2.5.1.5 OneTimeUse
        // Core 2.5.1.6 ProxyRestriction
    }
}