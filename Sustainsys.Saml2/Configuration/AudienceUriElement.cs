using System.Configuration;

namespace Sustainsys.Saml2.Configuration
{
	public class AudienceUriElement : ConfigurationElement
	{
		[ConfigurationProperty("Value", IsRequired = true, DefaultValue = " ", IsKey = true)]
		[StringValidator(MinLength = 1)]
		public string Value
		{
			get { return (string)this["Value"]; }
			set { this["Value"] = value; }
		}
	}
}
