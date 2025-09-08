using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Validation;
using System.Security.Claims;

namespace Sustainsys.Saml2;

/// <summary>
/// Default claims factory
/// </summary>
public class ClaimsFactory : IClaimsFactory
{
    /// <inheritdoc/>
    public ClaimsIdentity GetClaimsIdentity(Valid<Assertion> validatedAssertion)
    {
        List<Claim> claims = [];

        Assertion assertion = validatedAssertion;

        if (assertion.Subject?.NameId != null)
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
                    // TODO: Add support for xs:nil
                    claims.Add(new(attribute.Name, value!));
                }
            }
        }

        return new ClaimsIdentity(claims, "saml2");
    }
}