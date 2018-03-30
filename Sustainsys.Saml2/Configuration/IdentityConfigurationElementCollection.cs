using System;
using System.Configuration;

namespace Sustainsys.Saml2.Configuration
{
	[ConfigurationCollection(typeof(IdentityConfigurationElement), AddItemName = "identityConfiguration", CollectionType = ConfigurationElementCollectionType.BasicMap)]
	public class IdentityConfigurationElementCollection : ConfigurationElementCollection
	{
		protected override bool ThrowOnDuplicate => true;

		protected override ConfigurationElement CreateNewElement()
		{
			return new IdentityConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}
			if (element is IdentityConfigurationElement icElement)
			{
				return icElement.Name;
			}
			throw new InvalidOperationException(
				$"Invalid configuration element type {element.GetType()}");
		}

		protected override void BaseAdd(ConfigurationElement element)
		{
			string name = (string)GetElementKey(element);
			if (BaseGet(name) != null)
			{
				throw new InvalidOperationException(
					$"Duplicate identityConfiguration element with name '{name}'");
			}
			base.BaseAdd(element);
		}

		public IdentityConfigurationElement GetElement(string key)
		{
			return BaseGet(key) as IdentityConfigurationElement;
		}
	}
}
