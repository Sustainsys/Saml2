using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Owin;
using Kentor.AuthServices.Owin;
using FluentAssertions;

namespace Kentor.AuthServices.Tests.Owin
{
    [TestClass]
    public class KentorAuthServicesAuthenticationExtensionsTests
    {
        [TestMethod]
        public void KentorAuthServicesAuthenticationExtensions_UseKentorAuthServicesAuthentication()
        {
            var app = Substitute.For<IAppBuilder>();

            var options = new KentorAuthServicesAuthenticationOptions(true);

            app.UseKentorAuthServicesAuthentication(options);

            app.Received().Use(typeof(KentorAuthServicesAuthenticationMiddleware), app, options);
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationExtensions_UseKentorAuthServicesAuthentication_Nullcheck()
        {
            Action a = () => ((IAppBuilder)null).UseKentorAuthServicesAuthentication(null);

            a.ShouldThrow<ArgumentNullException>("app");
        }
    }
}
