using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Owin;
using Sustainsys.Saml2.Owin;
using FluentAssertions;

namespace Sustainsys.Saml2.Owin.Tests
{
    [TestClass]
    public class Saml2AuthenticationExtensionsTests
    {
        [TestMethod]
        public void Saml2AuthenticationExtensions_UseSaml2Authentication()
        {
            var app = Substitute.For<IAppBuilder>();

            var options = new Saml2AuthenticationOptions(true);

            app.UseSaml2Authentication(options);

            app.Received().Use(typeof(Saml2AuthenticationMiddleware), app, options);
        }

        [TestMethod]
        public void Saml2AuthenticationExtensions_UseSaml2Authentication_Nullcheck()
        {
            Action a = () => ((IAppBuilder)null).UseSaml2Authentication(null);

            a.Should().Throw<ArgumentNullException>("app");
        }
    }
}
