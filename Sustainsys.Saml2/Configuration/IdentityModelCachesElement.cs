using System.Configuration;

namespace Sustainsys.Saml2.Configuration
{
	public class IdentityModelCachesElement : ConfigurationElement
	{
		[ConfigurationProperty("tokenReplayCache")]
		public CustomTypeElement TokenReplayCache
		{
			get { return (CustomTypeElement)this["tokenReplayCache"]; }
			set { this["tokenReplayCache"] = value; }
		}
	}
}
