using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class KentorAuthServicesAuthenticationHandlerTests
    {
        [TestMethod]
        public void KentorAuthServicesAuthenticationHandler_RedirectsOnAuthChallenge()
        {
            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(404, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, null)),
                new KentorAuthServicesAuthenticationOptions());

            var context = new OwinContext();
            Action<Action<object>, object> onSendingHeaders = (Action, obj) => { };
            context.Environment["server.OnSendingHeaders"] = onSendingHeaders;

            middleware.Invoke(context).Wait();

            context.Response.StatusCode.Should().Be(302);
            context.Response.Headers["Location"].Should().StartWith("https://idp.example.com/idp");
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationHandler_NoRedirectOnNon404()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationHandler_NoRedirectWithoutChallenge()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationHandler_RedirectoToSecondIdp()
        {
            Assert.Inconclusive();
        }
    }
}
