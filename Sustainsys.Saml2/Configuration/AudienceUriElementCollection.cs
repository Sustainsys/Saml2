using Sustainsys.Saml2.Tokens;
using System.Configuration;

namespace Sustainsys.Saml2.Configuration
{
	[ConfigurationCollection(typeof(AudienceUriElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class AudienceUriElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement() =>
			new AudienceUriElement();

		protected override object GetElementKey(ConfigurationElement element) =>
			((AudienceUriElement)element).Value;

		[ConfigurationProperty("mode", DefaultValue = AudienceUriMode.Always)]
		public AudienceUriMode Mode
		{
			get { return (AudienceUriMode)this["mode"]; }
			set { this["mode"] = value; }
		}
	}
}
