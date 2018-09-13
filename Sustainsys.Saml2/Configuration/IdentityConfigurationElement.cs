using System;
using System.Configuration;

namespace Sustainsys.Saml2.Configuration
{
	public class IdentityConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("Name", IsKey = true)]
		public string Name
		{
			get { return (string)this["Name"]; }
			set { this["Name"] = value; }
		}

		[ConfigurationProperty("audienceUris")]
		public AudienceUriElementCollection AudienceUris
		{
			get { return (AudienceUriElementCollection)this["audienceUris"]; }
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

		[ConfigurationProperty("caches")]
		public IdentityModelCachesElement Caches
		{
			get { return (IdentityModelCachesElement)this["caches"]; }
			set { this["caches"] = value; }
		}

		[ConfigurationProperty("claimsAuthenticationManager")]
		public CustomTypeElement ClaimsAuthenticationManager
		{
			get { return (CustomTypeElement)this["claimsAuthenticationManager"]; }
			set { this["claimsAuthenticationManager"] = value; }
		}

		[ConfigurationProperty("tokenReplayDetection")]
		public TokenReplayDetectionElement TokenReplayDetection
		{
			get { return (TokenReplayDetectionElement)this["tokenReplayDetection"]; }
			set { this["tokenReplayDetection"] = value; }
		}

		[ConfigurationProperty("securityTokenHandlers")]
		public SecurityTokenHandlersConfigurationElement SecurityTokenHandlers
		{
			get { return (SecurityTokenHandlersConfigurationElement)this["securityTokenHandlers"]; }
		}
	}
}
