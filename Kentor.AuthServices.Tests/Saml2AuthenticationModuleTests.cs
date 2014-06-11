using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Xml.Linq;
using System.IdentityModel.Tokens;
using System.Xml;
using Kentor.AuthServices.Configuration;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2AuthenticationModuleTests
    {
        [TestMethod]
        public void ModulePath_Can_Be_Configured()
        {
          var modulePath = "~/MyCustomAuthSequence";
          KentorAuthServicesSection.Current.ModulePath = modulePath;

          var x = Saml2AuthenticationModule.ModulePath;
          x.Should().Be(modulePath);
        }
        
        [TestMethod]
        public void ModulePath_Is_Default_When_Not_Specified()
        {
            // Ensure that the value is non-existent
            KentorAuthServicesSection.Current.ModulePath = null;

            var x = Saml2AuthenticationModule.ModulePath;
          
            x.Should().Be("~/AuthServices");
        }
    }
}
