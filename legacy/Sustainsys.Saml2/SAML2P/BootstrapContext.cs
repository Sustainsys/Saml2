using Microsoft.IdentityModel.Tokens;

namespace Sustainsys.Saml2.Saml2P
{
	public class BootstrapContext
    {
		public SecurityTokenHandler SecurityTokenHandler { get; private set; }
		public SecurityToken SecurityToken { get; private set; }

		public BootstrapContext(SecurityToken token, SecurityTokenHandler handler)
		{
			SecurityToken = token;
			SecurityTokenHandler = handler;
		}

	}
}
