using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Sustainsys.Saml2.Internal;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Tokens;
using Sustainsys.Saml2.Saml2P;
using System.Diagnostics.CodeAnalysis;
using System.Collections.ObjectModel;

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

	public class CustomTypeElement : ConfigurationElement
	{
		[ConfigurationProperty("type", IsRequired = true, IsKey = true)]
		[TypeConverter(typeof(System.Configuration.TypeNameConverter))]
		public Type Type
		{
			get { return (Type)this["type"]; }
			set { this["type"] = value; }
		}
	}

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

#if FALSE
		[ConfigurationProperty(ConfigurationStrings.Caches, IsRequired = false)]
		public IdentityModelCachesElement Caches
		{
			get { return (IdentityModelCachesElement)this[ConfigurationStrings.Caches]; }
			set { this[ConfigurationStrings.Caches] = value; }
		}

		[ConfigurationProperty(ConfigurationStrings.X509CertificateValidation, IsRequired = false)]
		public X509CertificateValidationElement CertificateValidation
		{
			get { return (X509CertificateValidationElement)this[ConfigurationStrings.X509CertificateValidation]; }
			set { this[ConfigurationStrings.X509CertificateValidation] = value; }
		}
#endif
		[ConfigurationProperty("claimsAuthenticationManager")]
		public CustomTypeElement ClaimsAuthenticationManager
		{
			get { return (CustomTypeElement)this["claimsAuthenticationManager"]; }
			set { this["claimsAuthenticationManager"] = value; }
		}
		#if FALSE
		[ConfigurationProperty(ConfigurationStrings.ClaimsAuthorizationManager, IsRequired = false)]
		public CustomTypeElement ClaimsAuthorizationManager
		{
			get { return (CustomTypeElement)this[ConfigurationStrings.ClaimsAuthorizationManager]; }
			set { this[ConfigurationStrings.ClaimsAuthorizationManager] = value; }
		}

		[ConfigurationProperty(ConfigurationStrings.IssuerNameRegistry, IsRequired = false)]
		public IssuerNameRegistryElement IssuerNameRegistry
		{
			get { return (IssuerNameRegistryElement)this[ConfigurationStrings.IssuerNameRegistry]; }
			set { this[ConfigurationStrings.IssuerNameRegistry] = value; }
		}

		[ConfigurationProperty(ConfigurationStrings.IssuerTokenResolver, IsRequired = false)]
		public CustomTypeElement IssuerTokenResolver
		{
			get { return (CustomTypeElement)this[ConfigurationStrings.IssuerTokenResolver]; }
			set { this[ConfigurationStrings.IssuerTokenResolver] = value; }
		}

		[ConfigurationProperty(ConfigurationStrings.ServiceTokenResolver, IsRequired = false)]
		public CustomTypeElement ServiceTokenResolver
		{
			get { return (CustomTypeElement)this[ConfigurationStrings.ServiceTokenResolver]; }
			set { this[ConfigurationStrings.ServiceTokenResolver] = value; }
		}

		[ConfigurationProperty(ConfigurationStrings.TokenReplayDetection, IsRequired = false)]
		public TokenReplayDetectionElement TokenReplayDetection
		{
			get { return (TokenReplayDetectionElement)this[ConfigurationStrings.TokenReplayDetection]; }
			set { this[ConfigurationStrings.TokenReplayDetection] = value; }
		}
		#endif

		[ConfigurationProperty("securityTokenHandlers")]
		public SecurityTokenHandlersConfigurationElement SecurityTokenHandlers
		{
			get { return (SecurityTokenHandlersConfigurationElement)this["securityTokenHandlers"]; }
		}
	}

	public class SecurityTokenHandlersConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("", IsDefaultCollection = true)]
		public SecurityTokenHandlerConfigurationElementCollection SecurityTokenHandlersCollection
		{
			get { return (SecurityTokenHandlerConfigurationElementCollection)this[""]; }
		}
	}

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

	public class IdentityConfigurationsElement : ConfigurationElement
	{
		[ConfigurationProperty("", IsDefaultCollection = true)]
		public IdentityConfigurationElementCollection IdentityConfigurationsCollection
		{
			get { return (IdentityConfigurationElementCollection)this[""]; }
		}
	}

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
