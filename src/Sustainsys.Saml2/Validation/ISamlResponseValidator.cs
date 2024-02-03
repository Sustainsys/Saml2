using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Samlp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Validation;

/// <summary>
/// Validates a Saml response
/// </summary>
public interface ISamlResponseValidator
{
    /// <summary>
    /// Validates a Saml response.
    /// </summary>
    /// <param name="samlResponse"></param>
    /// <param name="parameters">Expected values and settings for validation</param>
    void Validate(SamlResponse samlResponse, SamlResponseValidationParameters parameters);
}


/// <summary>
/// DTO carrying parameters for Saml response validation.
/// </summary>
public class SamlResponseValidationParameters
{
    /// <summary>
    /// Valid issuer of the response and assertions
    /// </summary>
    public NameId? ValidIssuer {  get; set; }
}