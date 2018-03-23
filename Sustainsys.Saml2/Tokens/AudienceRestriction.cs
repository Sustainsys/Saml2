using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Sustainsys.Saml2.Tokens
{
	public enum AudienceUriMode
	{
		Always,
		BearerKeyOnly,
		Never
	}

	public class AudienceRestriction
    {
		public AudienceUriMode AudienceMode { get; set; } = AudienceUriMode.Always;
		public Collection<Uri> AllowedAudienceUris { get; } = new Collection<Uri>();

		public AudienceRestriction()
		{
		}

		public AudienceRestriction(AudienceUriMode audienceMode)
		{
			AudienceMode = audienceMode;
		}
	}
}
