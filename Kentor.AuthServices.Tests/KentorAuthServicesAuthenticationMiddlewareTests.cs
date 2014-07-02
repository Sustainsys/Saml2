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
using System.Web;
using Kentor.AuthServices.TestHelpers;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class KentorAuthServicesAuthenticationMiddlewareTests
    {
        class ProtectedCaller : KentorAuthServicesAuthenticationMiddleware
        {
            public ProtectedCaller(OwinMiddleware next,
                KentorAuthServicesAuthenticationOptions options)
                : base(next, options)
            { }

            public AuthenticationHandler<KentorAuthServicesAuthenticationOptions> CallCreateHandler()
            {
                return CreateHandler();
            }
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_CtorNullChecksOptions()
        {
            Action a = () => new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(0, null),
                null);

            a.ShouldThrow<ArgumentNullException>("options");
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
            var authenticationType = "someAuthName";

            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[] { authenticationType }, null)),
                new KentorAuthServicesAuthenticationOptions()
                {
                    AuthenticationType = authenticationType
                });

            var context = CreateOwinContext();

            middleware.Invoke(context).Wait();

            context.Response.StatusCode.Should().Be(302);
            context.Response.Headers["Location"].Should().StartWith("https://idp.example.com/idp");
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationMiddleware_RedirectRemembersReturnPath()
        {
            var returnUri = "http://sp.example.com/returnuri";

            var middleware = new KentorAuthServicesAuthenticationMiddleware(
                new StubOwinMiddleware(401, new AuthenticationResponseChallenge(
                    new string[] { "KentorAuthServices" }, new AuthenticationProperties()
                    {
                        RedirectUri = returnUri
                    })),
                    new KentorAuthServicesAuthenticationOptions());

            var context = CreateOwinContext();

            middleware.Invoke(context).Wait();

            var requestId = AuthnRequestHelper.GetRequestId(new Uri(context.Response.Headers["Location"]));

            StoredRequestState storedAuthnData;
            PendingAuthnRequests.TryRemove(new System.IdentityModel.Tokens.Saml2Id(requestId), out storedAuthnData);

            storedAuthnData.ReturnUri.Should().Be(returnUri);
        }
    }
}
