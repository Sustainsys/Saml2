using Kentor.AuthServices;
using Kentor.AuthServices.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SampleAspNetCoreApplication
{
    public class KentorAuthServicesUserClaimsPrincipalFactory<TUser, TRole>
        : UserClaimsPrincipalFactory<TUser, TRole>, IUserClaimsPrincipalFactory<TUser>
        where TUser : class
        where TRole : class
    {
        public KentorAuthServicesUserClaimsPrincipalFactory(
            UserManager<TUser> userManager,
            RoleManager<TRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            IHttpContextAccessor httpContextAccessor,
            IOptions<KentorAuthServicesOptions> kentorOptionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
            HttpContext = httpContextAccessor.HttpContext;
            KentorOptions = kentorOptionsAccessor.Value;
        }

        protected HttpContext HttpContext { get; set; }
        protected KentorAuthServicesOptions KentorOptions { get; set; }

        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            var principal = await base.CreateAsync(user);
            var externalIdentity = await HttpContext.Authentication.AuthenticateAsync(Options.Cookies.ExternalCookieAuthenticationScheme);
            var sessionIdClaim = externalIdentity?.FindFirst(AuthServicesClaimTypes.SessionIndex);
            var externalNameIdClaim = externalIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if(principal == null || externalIdentity == null || sessionIdClaim == null || externalNameIdClaim == null)
            {
                return principal;
            }

            var identity = (ClaimsIdentity)principal.Identity;

            identity.AddClaim(new Claim(
                sessionIdClaim.Type,
                sessionIdClaim.Value,
                sessionIdClaim.ValueType,
                sessionIdClaim.Issuer));

            var logoutNameIdClaim = new Claim(
                AuthServicesClaimTypes.LogoutNameIdentifier,
                externalNameIdClaim.Value,
                externalNameIdClaim.ValueType,
                externalNameIdClaim.Issuer);

            foreach(var kv in externalNameIdClaim.Properties)
            {
                logoutNameIdClaim.Properties.Add(kv);
            }

            identity.AddClaim(logoutNameIdClaim);

            return principal;
        }
    }
}
