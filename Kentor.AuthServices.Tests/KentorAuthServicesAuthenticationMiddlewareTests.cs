using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Owin;
using FluentAssertions;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class KentorAuthServicesAuthenticationMiddlewareTests
    {
        class ProtectedCaller : KentorAuthServicesAuthenticationMiddleware
        {
            public ProtectedCaller(OwinMiddleware next,
                KentorAuthServicesAuthenticationOptions options)
                : base (next, options)
            {}

            public AuthenticationHandler<KentorAuthServicesAuthenticationOptions> CallCreateHandler()
            {
                return CreateHandler();
            }
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_RedirectsOnAuthChallenge()
        {
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, null)),
                new KentorAuthServicesAuthenticationOptions());

            var context = CreateOwinContext();

            middleware.Invoke(context).Wait();

            context.Response.StatusCode.Should().Be(302);
            context.Response.Headers["Location"].Should().StartWith("https://idp.example.com/idp");
        }

        private static OwinContext CreateOwinContext()
        {
            var context = new OwinContext();
            Action<Action<object>, object> onSendingHeaders = (Action, obj) => { };
            context.Environment["server.OnSendingHeaders"] = onSendingHeaders;
            context.Environment["owin.RequestScheme"] = "http";
            context.Request.Host = new HostString("sp.example.com");
            context.Request.Path = new PathString("/");
            return context;
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_NoRedirectOnNon401()
        {
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(200, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, null)),
                new KentorAuthServicesAuthenticationOptions());

            var context = CreateOwinContext();

            middleware.Invoke(context).Wait();

            context.Response.StatusCode.Should().Be(200);
            context.Response.Headers["Location"].Should().BeNull();
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_NoRedirectWithoutChallenge()
        {
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, null),
                new KentorAuthServicesAuthenticationOptions());

            var context = CreateOwinContext();

            middleware.Invoke(context).Wait();

            context.Response.StatusCode.Should().Be(401);
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_RedirectoToSecondIdp()
        {
            var secondIdp = IdentityProvider.ConfiguredIdentityProviders.Skip(1).First().Value;
            var secondDestination = secondIdp.DestinationUri;
            var secondEntityId = secondIdp.Issuer;

            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, new AuthenticationProperties(
                        new Dictionary<string, string>()
                        {
                            { "idp", secondEntityId }
                        }))),
                        new KentorAuthServicesAuthenticationOptions());

            var context = CreateOwinContext();
            middleware.Invoke(context).Wait();

            context.Response.StatusCode.Should().Be(302);
            context.Response.Headers["Location"].Should().StartWith(secondDestination.ToString());
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_RedirectOnChallengeForAuthTypeInOptions()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_RedirectRemembersReturnPath()
        {
            Assert.Inconclusive();
        }
    }
}
