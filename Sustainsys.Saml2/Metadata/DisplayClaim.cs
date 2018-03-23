using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Sustainsys.Saml2.Metadata
{
	public class DisplayClaim
	{
		public string ClaimType { get; private set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }
//?		public string DisplayTag { get; set; }
		public string DisplayValue { get; set; }
		public bool? Optional { get; set; }
		public bool WriteOptionalAttribute { get; set; }
		public string Value { get; set; }
		public XmlElement StructuredValue { get; set; }
		public EncryptedValue EncryptedValue { get; set; }
		public ConstrainedValue ConstrainedValue { get; set; }

		public static DisplayClaim CreateDisplayClaimFromClaimType(string claimType)
		{
			// TODO: map ClaimTypes.Claim to some display text
			// do we need this?
			return new DisplayClaim(claimType);
		}
#if FALSE

		public DisplayClaim(string claimType, string displayTag, string description, string displayValue, bool optional)
		{
			ClaimType = claimType;
			DisplayTag = displayTag;
			Description = description;
			DisplayValue = displayValue;
			Optional = optional;
		}

		public DisplayClaim(string claimType, string displayTag, string description, string displayValue) :
			this(claimType, displayTag, description, displayValue, true)
		{
		}
		public DisplayClaim(string claimType, string displayTag, string description) :
			this(claimType, displayTag, description, null)
		{
		}
#endif

		public DisplayClaim(string claimType) //:
			//this(claimType, null, null)
		{
			if (claimType == null)
			{
				throw new ArgumentNullException(nameof(claimType));
			}
			ClaimType = claimType;
		}
	}
}
