using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace Sustainsys.Saml2.Metadata
{
    public class LocalizedName : LocalizedEntry
    {
		public string Name { get; set; }

		public LocalizedName(string name, string language) :
			base(language)
		{
			Name = name;
		}

		public LocalizedName()
		{
		}
    }
}
