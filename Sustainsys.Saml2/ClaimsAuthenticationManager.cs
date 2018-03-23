using Sustainsys.Saml2.Configuration;
using System;
using System.Security.Claims;
using System.Xml;

namespace Sustainsys.Saml2
{
	public class ClaimsAuthenticationManager : ICustomIdentityConfiguration
	{
		public virtual ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
		{
			return incomingPrincipal;
		}

		public virtual void LoadCustomConfiguration(XmlNodeList nodelist)
		{
			throw new NotImplementedException();
		}
	}
}
