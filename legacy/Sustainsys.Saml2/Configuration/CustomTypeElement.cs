using System;
using System.ComponentModel;
using System.Configuration;

namespace Sustainsys.Saml2.Configuration
{
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
}
