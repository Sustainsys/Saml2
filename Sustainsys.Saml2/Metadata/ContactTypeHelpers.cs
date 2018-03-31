using System;
using System.Collections.Generic;
using System.Text;

namespace Sustainsys.Saml2.Metadata
{
    public static class ContactTypeHelpers
    {
		public static ContactType ParseContactType(string contactType)
		{
			if (contactType == null)
			{
				throw new ArgumentNullException(nameof(contactType));
			}

			switch (contactType)
			{
				case "technical":
					return ContactType.Technical;
				case "support":
					return ContactType.Support;
				case "administrative":
					return ContactType.Administrative;
				case "billing":
					return ContactType.Billing;
				case "other":
					return ContactType.Other;
				default:
					throw new FormatException($"Unknown contactType value '{contactType}'");
			}
		}
	}
}
