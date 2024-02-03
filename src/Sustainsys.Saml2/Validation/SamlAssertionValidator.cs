using Sustainsys.Saml2.Saml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Validation;

/// <summary>
/// Saml Assertion validator
/// </summary>
public class SamlAssertionValidator : ISamlAssertionValidator
{
    /// <inheritdoc/>
    public void Validate(
        SamlAssertion assertion,
        SamlAssertionValidationParameters parameters)
    {
        // TODO: Remember to validate issuer.
    }
}
