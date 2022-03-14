using System;
using System.Configuration;

namespace Sustainsys.Saml2.Configuration
{
	public class TokenReplayDetectionElement : ConfigurationElement
	{
		[ConfigurationProperty("enabled", DefaultValue = true)]
		public bool Enabled
		{
			get { return (bool)this["enabled"]; }
			set { this["enabled"] = value; }
		}

		[ConfigurationProperty("expirationPeriod", DefaultValue = "10675199.02:48:05.4775807")]
		public TimeSpan ExpirationPeriod
		{
			get { return (TimeSpan)this["expirationPeriod"]; }
			set { this["expirationPeriod"] = value; }
		}
	}
}
