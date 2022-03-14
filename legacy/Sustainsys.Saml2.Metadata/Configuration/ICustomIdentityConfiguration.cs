using System.Xml;

namespace Sustainsys.Saml2.Metadata.Configuration
{
    interface ICustomIdentityConfiguration
    {
		void LoadCustomConfiguration(XmlNodeList nodeList);
	}
}
