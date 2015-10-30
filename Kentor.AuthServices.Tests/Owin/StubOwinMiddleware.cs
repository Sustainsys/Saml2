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

        public StubOwinMiddleware(int statusCode, AuthenticationResponseChallenge challenge)
             : base(null)
        {
            this.statusCode = statusCode;
            this.challenge = challenge;
        }

        public override Task Invoke(IOwinContext context)
        {
            context.Response.StatusCode = statusCode;
            context.Authentication.AuthenticationResponseChallenge = challenge;
            context.Response.ContentLength = 0;
            return Task.FromResult(0);
        }
    }
}
