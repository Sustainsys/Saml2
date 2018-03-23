using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Xml;

namespace Sustainsys.Saml2.Configuration
{
    public abstract class IssuerNameRegistry : ICustomIdentityConfiguration
    {
		public abstract string GetIssuerName(SecurityToken securityToken);

		public virtual string GetIssuerName(SecurityToken securityToken, string requestedIssuerName)
		{
			return GetIssuerName(securityToken);
		}

		public virtual string GetWindowsIssuerName()
		{
			return ClaimsIdentity.DefaultIssuer;
		}

		public virtual void LoadCustomConfiguration(XmlNodeList nodeList)
		{
			throw new NotImplementedException("LoadCustomConfiguration is not implemented");
		}
	}
}
