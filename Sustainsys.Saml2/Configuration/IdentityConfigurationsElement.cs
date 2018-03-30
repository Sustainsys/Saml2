using System.Configuration;

namespace Sustainsys.Saml2.Configuration
{
	public class IdentityConfigurationsElement : ConfigurationElement
	{
		[ConfigurationProperty("", IsDefaultCollection = true)]
		public IdentityConfigurationElementCollection IdentityConfigurationsCollection
		{
			get { return (IdentityConfigurationElementCollection)this[""]; }
		}
	}
}
