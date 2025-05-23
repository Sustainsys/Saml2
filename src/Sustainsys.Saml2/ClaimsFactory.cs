using Sustainsys.Saml2.Saml;
using System.Security.Claims;

namespace Sustainsys.Saml2;

public class ClaimsFactory : IClaimsFactory
{
    public ClaimsIdentity GetClaimsIdentity(Assertion assertion)
    {
        List<Claim> claims = new();

        if (assertion.Subject.NameId != null)
        {
            claims.Add(new(ClaimTypes.NameIdentifier, assertion.Subject.NameId!.Value));
        }

        return new ClaimsIdentity(claims, "saml2");
    }
}
