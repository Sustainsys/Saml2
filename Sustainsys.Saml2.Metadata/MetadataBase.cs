using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sustainsys.Saml2.Metadata
{
    public abstract class MetadataBase
    {
		public SigningCredentials SigningCredentials { get; set; }
	}
}
