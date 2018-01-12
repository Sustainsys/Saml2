using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Owin;
using Sustainsys.Saml2.Owin;
using FluentAssertions;

namespace Sustainsys.Saml2.Owin.Tests
{
    [TestClass]
    public class SustainsysSaml2AuthenticationExtensionsTests
    {
        [TestMethod]
        public void SustainsysSaml2AuthenticationExtensions_UseSustainsysSaml2Authentication()
        {
            var app = Substitute.For<IAppBuilder>();

            var options = new SustainsysSaml2AuthenticationOptions(true);

            app.UseSustainsysSaml2Authentication(options);

            app.Received().Use(typeof(SustainsysSaml2AuthenticationMiddleware), app, options);
        }

        [TestMethod]
        public void SustainsysSaml2AuthenticationExtensions_UseSustainsysSaml2Authentication_Nullcheck()
        {
            Action a = () => ((IAppBuilder)null).UseSustainsysSaml2Authentication(null);

            a.ShouldThrow<ArgumentNullException>("app");
        }
    }
}
