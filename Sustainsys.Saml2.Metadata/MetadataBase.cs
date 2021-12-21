using Microsoft.IdentityModel.Tokens;

namespace Sustainsys.Saml2.Metadata
{
    public abstract class MetadataBase
    {
        public SigningCredentials SigningCredentials { get; set; }
    }
}