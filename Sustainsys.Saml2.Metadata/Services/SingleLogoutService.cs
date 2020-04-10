using System;

namespace Sustainsys.Saml2.Metadata.Services
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