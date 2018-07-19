using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Owin;
using FluentAssertions;
using Microsoft.Owin.Security;
using System.IdentityModel.Metadata;

namespace Sustainsys.Saml2.Owin.Tests
{
    [TestClass]
    public class Saml2AuthenticationOptionsTests
    {
        [TestMethod]
        public void Saml2AuthenticationOptions_Ctor_SetsDefault()
        {
            var subject = new Saml2AuthenticationOptions(true);

            subject.Description.Caption.Should().Be(Constants.DefaultCaption);
            subject.AuthenticationMode.Should().Be(AuthenticationMode.Passive);
        }

        [TestMethod]
        public void Saml2AuthenticationOptions_Ctor_LoadsConfiguration()
        {
            var subject = new Saml2AuthenticationOptions(true);

            subject.SPOptions.EntityId.Id.Should().Be("https://github.com/SustainsysIT/Saml2");

            subject.IdentityProviders.IsEmpty.Should().BeFalse();
            subject.IdentityProviders[new EntityId("https://idp.example.com")]
                .SingleSignOnServiceUrl.Should().Be("https://idp.example.com/idp");
        }

        [TestMethod]
        public void Saml2AuthenticationOptions_Ctor_IgnoresConfiguration()
        {
            var subject = new Saml2AuthenticationOptions(false);

            subject.SPOptions.Should().BeNull();
            subject.IdentityProviders.IsEmpty.Should().BeTrue();
        }

        [TestMethod]
        public void Saml2AuthenticationOptions_Ctor_LoadsIdpFromConfiguration()
        {
            var subject = new Saml2AuthenticationOptions(true);

            subject.IdentityProviders.Default.EntityId.Id.Should().Be("https://idp.example.com");
        }

        [TestMethod]
        public void Saml2AuthenticationOptions_Ctor_LoadsFederationFromConfigurationAndRegistersIdp()
        {
            var subject = new Saml2AuthenticationOptions(true);

            Action a = () =>
            {
                var i = subject.IdentityProviders[new EntityId("http://idp.federation.example.com/metadata")];
            };

            a.ShouldNotThrow();
        }

        [TestMethod]
        public void Saml2AuthenticationOptions_Caption()
        {
            var subject = new Saml2AuthenticationOptions(false)
            {
                Caption = "MyCaption"
            };

            subject.Caption.Should().Be("MyCaption");
            subject.Description.Caption.Should().Be("MyCaption");
        }
    }
}
