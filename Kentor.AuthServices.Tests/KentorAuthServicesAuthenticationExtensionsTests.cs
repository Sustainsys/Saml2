using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Owin;
using Kentor.AuthServices.Owin;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class KentorAuthServicesAuthenticationExtensionsTests
    {
        [TestMethod]
        public void KentorAuthServicesAuthenticationExtensions_UseKentorAuthServicesAuthentication()
        {
            var app = Substitute.For<IAppBuilder>();

            app.UseKentorAuthServicesAuthentication();

            app.Received().Use(typeof(KentorAuthServicesAuthenticationMiddleware));
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationExtensions_UseKentorAuthServicesAuthentication_Nullcheck()
        {
            Action a = () => ((IAppBuilder)null).UseKentorAuthServicesAuthentication();

            a.ShouldThrow<ArgumentNullException>("app");
        }
    }
}
