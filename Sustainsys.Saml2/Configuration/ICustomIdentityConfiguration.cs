using System.Xml;

namespace Sustainsys.Saml2.Configuration
{
    interface ICustomIdentityConfiguration
    {
		void LoadCustomConfiguration(XmlNodeList nodeList);
	}
}
