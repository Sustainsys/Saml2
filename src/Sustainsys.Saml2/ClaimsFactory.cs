using Sustainsys.Saml2.Saml;
using System.Security.Claims;

namespace Sustainsys.Saml2;

/// <summary>
/// Default claims factory
/// </summary>
public class ClaimsFactory : IClaimsFactory
{
    /// <inheritdoc/>
    public ClaimsIdentity GetClaimsIdentity(Assertion assertion)
    {
        List<Claim> claims = new();

        if (assertion.Subject.NameId != null)
        {
            // TODO: Make Claim Type Configurable.
            // TODO: Add Claims issuer (configurable?)
            claims.Add(new(ClaimTypes.NameIdentifier, assertion.Subject.NameId!.Value));
        }

        if (assertion.Attributes != null)
        {
            foreach (var attribute in assertion.Attributes)
            {
                foreach (var value in attribute.Values)
                {
                    if (value != null)
                    {
                        claims.Add(new(attribute.Name, value));
                    }
                }
            }
        }

        return new ClaimsIdentity(claims, "saml2");
    }
}