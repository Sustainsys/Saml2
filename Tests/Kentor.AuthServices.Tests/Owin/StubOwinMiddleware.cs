using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Tests.Owin
{
    class StubOwinMiddleware : OwinMiddleware
    {
        private int statusCode;
        private AuthenticationResponseChallenge challenge;
        private AuthenticationResponseRevoke revoke;
        private AuthenticationResponseGrant grant;

        public StubOwinMiddleware(
            int statusCode,
            AuthenticationResponseChallenge challenge = null,
            AuthenticationResponseRevoke revoke = null,
            AuthenticationResponseGrant grant = null)
             : base(null)
        {
            this.statusCode = statusCode;
            this.challenge = challenge;
            this.revoke = revoke;
            this.grant = grant;
        }

        public override Task Invoke(IOwinContext context)
        {
            context.Response.StatusCode = statusCode;
            context.Authentication.AuthenticationResponseChallenge = challenge;
            context.Authentication.AuthenticationResponseRevoke = revoke;
            context.Authentication.AuthenticationResponseGrant = grant;
            context.Response.ContentLength = 0;
            return Task.FromResult(0);
        }
    }
}
