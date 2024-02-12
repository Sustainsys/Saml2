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
        // Core 2.5.1
        ValidateConditions(samlResponse, validationParameters);
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
    protected virtual void ValidateStatusCode(SamlResponse samlResponse)
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

    /// <summary>
    /// Validate the version
    /// </summary>
    /// <param name="samlResponse">Saml response</param>
    /// <exception cref="SamlValidationException">On validation failure</exception>
    protected virtual void ValidateVersion(SamlResponse samlResponse)
    {
        if (samlResponse.Version != "2.0")
        {
            throw new SamlValidationException($"Saml version \"{samlResponse.Version}\" is incorrect, it must be exactly \"2.0\"");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="samlResponse">Saml response</param>
    /// <param name="validationParameters">Validation parameters</param>
    /// <exception cref="SamlValidationException">On validation failure</exception>
    protected virtual void ValidateConditions(
        SamlResponse samlResponse,
        SamlResponseValidationParameters validationParameters)
    {
        // Core 2.5.1.2 NotBefore, NotOnOrAfter
        // Core 2.5.1.4 AudienceRestriction, Audience
        // Core 2.5.1.5 OneTimeUse
        // Core 2.5.1.6 ProxyRestriction
    }

}