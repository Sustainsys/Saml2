using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;

namespace Kentor.AuthServices.AspNetCore
{
    class MultipleIdentityAuthenticationTicket : AuthenticationTicket
    {
        public MultipleIdentityAuthenticationTicket(
            ClaimsPrincipal principal,
            AuthenticationProperties properties,
            KentorAuthServicesOptions options) :
            base(principal, properties, options.AuthenticationScheme)
        {
            this.identities = principal.Identities.Select(i =>
                new ClaimsIdentity(i, null, options.SignInAsAuthenticationType, i.NameClaimType, i.RoleClaimType));
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
