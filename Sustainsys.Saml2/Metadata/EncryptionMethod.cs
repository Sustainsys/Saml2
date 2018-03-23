using System;

namespace Sustainsys.Saml2.Metadata
{
	public class EncryptionMethod
	{
		public int KeySize { get; set; }
		public string OAEPparams { get; set; }
		public Uri Algorithm { get; set; }
	}
}
