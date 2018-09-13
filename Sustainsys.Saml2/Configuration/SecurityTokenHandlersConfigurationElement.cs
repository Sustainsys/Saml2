using System.Configuration;

namespace Sustainsys.Saml2.Configuration
{
	public class SecurityTokenHandlersConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("", IsDefaultCollection = true)]
		public SecurityTokenHandlerConfigurationElementCollection SecurityTokenHandlersCollection
		{
			get { return (SecurityTokenHandlerConfigurationElementCollection)this[""]; }
		}
	}
}
