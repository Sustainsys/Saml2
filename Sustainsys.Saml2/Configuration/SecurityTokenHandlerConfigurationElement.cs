using System;
using System.Configuration;

namespace Sustainsys.Saml2.Configuration
{
	public class SecurityTokenHandlerConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("Name", IsKey = true)]
		public string Name
		{
			get { return (string)this["Name"]; }
			set { this["Name"] = value; }
		}

		[ConfigurationProperty("saveBootstrapContext")]
		public bool SaveBootstrapContext
		{
			get { return (bool)this["saveBootstrapContext"]; }
			set { this["saveBootstrapContext"] = value; }
		}

		[ConfigurationProperty("maximumClockSkew", DefaultValue = "00:05:00")]
		public TimeSpan MaximumClockSkew
		{
			get { return (TimeSpan)this["maximumClockSkew"]; }
			set { this["maximumClockSkew"] = value; }
		}

		[ConfigurationProperty("audienceUris")]
		public AudienceUriElementCollection AudienceUris
		{
			get { return (AudienceUriElementCollection)this["audienceUris"]; }
		}
	}
}
