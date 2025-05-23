using Sustainsys.Saml2.Saml;
using System.Security.Claims;

namespace Sustainsys.Saml2;

public interface IClaimsFactory
{
    ClaimsIdentity GetClaimsIdentity(Assertion assertion);
}
