using System;

namespace Sustainsys.Saml2.Metadata
{
	public class SingleLogoutService : Endpoint
	{
		public SingleLogoutService()
		{
		}

		public SingleLogoutService(Uri binding, Uri location) :
			base(binding, location)
		{
		}
	}
}
