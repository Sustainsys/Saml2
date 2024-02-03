using Sustainsys.Saml2.Saml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Validation;

/// <summary>
/// Validates an asseriton
/// </summary>
public interface ISamlAssertionValidator
{
    /// <summary>
    /// Validate a Saml assertion
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="parameters"></param>
    void Validate(SamlAssertion assertion, SamlAssertionValidationParameters parameters);
}

/// <summary>
/// DTO carrying parameters for Saml assertion validation
/// </summary>
public class SamlAssertionValidationParameters
{
    /// <summary>
    /// Valid issuer of the response and assertions
    /// </summary>
    public NameId? ValidIssuer { get; set; }
}