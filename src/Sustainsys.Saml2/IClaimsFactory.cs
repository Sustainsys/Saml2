using Sustainsys.Saml2.Saml;
using System.Security.Claims;

namespace Sustainsys.Saml2;

/// <summary>
/// Factory that creates a claims identity from a Saml2 Assertion.
/// </summary>
public interface IClaimsFactory
{
    /// <summary>
    /// Create a <see cref="ClaimsIdentity"/> from an
    /// <see cref="Assertion"/>.
    /// </summary>
    /// <param name="assertion">Source data</param>
    /// <returns>ClaimsIdentity</returns>
    ClaimsIdentity GetClaimsIdentity(Assertion assertion);
}
