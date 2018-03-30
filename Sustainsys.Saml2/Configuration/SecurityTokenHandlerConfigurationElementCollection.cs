using System;
using System.Configuration;

namespace Sustainsys.Saml2.Configuration
{
	[ConfigurationCollection(typeof(SecurityTokenHandlerConfigurationElement), AddItemName = "securityTokenHandlerConfiguration", CollectionType = ConfigurationElementCollectionType.BasicMap)]
	public class SecurityTokenHandlerConfigurationElementCollection : ConfigurationElementCollection
	{
		protected override bool ThrowOnDuplicate => true;

		protected override ConfigurationElement CreateNewElement()
		{
			return new SecurityTokenHandlerConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}
			if (element is SecurityTokenHandlerConfigurationElement sthElement)
			{
				return sthElement.Name;
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
					$"Duplicate securityTokenHandler element with name '{name}'");
			}
			base.BaseAdd(element);
		}

		public SecurityTokenHandlerConfigurationElement GetElement(string key)
		{
			return BaseGet(key) as SecurityTokenHandlerConfigurationElement;
		}
	}
}
