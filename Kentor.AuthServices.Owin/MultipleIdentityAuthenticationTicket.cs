using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Owin
{
    class MultipleIdentityAuthenticationTicket : AuthenticationTicket
    {
        public MultipleIdentityAuthenticationTicket(IEnumerable<ClaimsIdentity> identities,
            AuthenticationProperties properties) :
            base(identities.FirstOrDefault(), properties)
        {
            this.identities = identities;
        }

        private IEnumerable<ClaimsIdentity> identities;

        public IEnumerable<ClaimsIdentity> Identities
        {
            get
            {
                return identities;
            }
        }
    }
}
