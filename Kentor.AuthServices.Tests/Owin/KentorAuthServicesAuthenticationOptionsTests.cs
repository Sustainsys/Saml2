using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Owin;
using FluentAssertions;
using Microsoft.Owin.Security;
using System.IdentityModel.Metadata;

namespace Kentor.AuthServices.Tests.Owin
{
    [TestClass]
    public class KentorAuthServicesAuthenticationOptionsTests
    {
        [TestMethod]
        public void KentorAuthServicesAuthenticationOptions_Ctor_SetsDefault()
        {
            var subject = new KentorAuthServicesAuthenticationOptions(true);

            subject.Description.Caption.Should().Be(Constants.DefaultCaption);
            subject.AuthenticationMode.Should().Be(AuthenticationMode.Active);
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationOptions_Ctor_LoadsConfiguration()
        {
            var subject = new KentorAuthServicesAuthenticationOptions(true);

            subject.SPOptions.EntityId.Id.Should().Be("https://github.com/KentorIT/authservices");

            subject.IdentityProviders.IsEmpty.Should().BeFalse();
            subject.IdentityProviders[new EntityId("https://idp.example.com")]
                .SingleSignOnServiceUrl.Should().Be("https://idp.example.com/idp");
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationOptions_Ctor_IgnoresConfiguration()
        {
            var subject = new KentorAuthServicesAuthenticationOptions(false);

            subject.SPOptions.Should().BeNull();
            subject.IdentityProviders.IsEmpty.Should().BeTrue();
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationOptions_Ctor_LoadsIdpFromConfiguration()
        {
            var subject = new KentorAuthServicesAuthenticationOptions(true);

            subject.IdentityProviders.Default.EntityId.Id.Should().Be("https://idp.example.com");
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationOptions_Ctor_LoadsFederationFromConfigurationAndRegistersIdp()
        {
            var subject = new KentorAuthServicesAuthenticationOptions(true);

            Action a = () =>
            {
                var i = subject.IdentityProviders[new EntityId("http://idp.federation.example.com/metadata")];
            };

            a.ShouldNotThrow();
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationOptions_Caption()
        {
            var subject = new KentorAuthServicesAuthenticationOptions(false)
            {
                Caption = "MyCaption"
            };

            subject.Caption.Should().Be("MyCaption");
            subject.Description.Caption.Should().Be("MyCaption");
        }
    }
}
