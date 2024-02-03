using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Saml;

/// <summary>
/// A Saml assertion
/// </summary>
public class SamlAssertion
{
    /// <summary>
    /// Issuer of the assertion.
    /// </summary>
    public NameId Issuer { get; set; } = default!;
}
