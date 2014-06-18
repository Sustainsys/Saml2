using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Owin;
using FluentAssertions;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin;
using Owin;

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
        public void KentorAuthServicesAuthenticationMiddleware_CreateHandler()
        {
            (new ProtectedCaller(null, new KentorAuthServicesAuthenticationOptions())).CallCreateHandler()
                .Should().BeOfType<KentorAuthServicesAuthenticationHandler>();
        }
    }
}
