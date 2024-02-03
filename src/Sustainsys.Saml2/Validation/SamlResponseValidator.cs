using Microsoft.Extensions.Logging;
using Sustainsys.Saml2.Samlp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Validation;

/// <summary>
/// Validates a Saml Response
/// </summary>
public class SamlResponseValidator : ISamlResponseValidator
{
    /// <inheritdoc/>
    public void Validate(
        SamlResponse samlResponse,
        SamlResponseValidationParameters validationParameters)
    {
        ValidateIssuer(samlResponse, validationParameters);
        ValidateStatusCode(samlResponse);
    }

    /// <summary>
    /// Validate that the status code is <see cref="Constants.StatusCodes.Success"/>
    /// </summary>
    /// <param name="samlResponse">Saml Response</param>
    public virtual void ValidateStatusCode(SamlResponse samlResponse)
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
    public virtual void ValidateIssuer(
        SamlResponse samlResponse,
        SamlResponseValidationParameters validationParameters)
    {
        if (samlResponse.Issuer != null &&
            samlResponse.Issuer != validationParameters.ValidIssuer)
        {
            throw new SamlValidationException(
                $"Response issuer {samlResponse.Issuer} does not match expected {validationParameters.ValidIssuer}");
        }
    }
}