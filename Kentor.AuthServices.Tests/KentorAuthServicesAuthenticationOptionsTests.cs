using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Owin;
using FluentAssertions;
using Microsoft.Owin.Security;
using System.IdentityModel.Metadata;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class KentorAuthServicesAuthenticationOptionsTests
    {
        [TestMethod]
        public void KentorAuthServicesAuthenticationOptions_Ctor_SetsDefault()
        {
            var subject = new KentorAuthServicesAuthenticationOptions(true);

            subject.Description.Caption.Should().Be(Constants.DefaultCaption);
            subject.AuthenticationMode.Should().Be(AuthenticationMode.Passive);
            subject.MetadataPath.ToString().Should().Be("/AuthServices");
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationOptions_Ctor_LoadsConfiguration()
        {
            var subject = new KentorAuthServicesAuthenticationOptions(loadConfiguration: true);

            subject.SPOptions.EntityId.Id.Should().Be("https://github.com/KentorIT/authservices");

            subject.IdentityProviders.Should().NotBeEmpty();
            subject.IdentityProviders[new EntityId("http://idp.example.com")]
                .SingleSignOnServiceUrl.Should().Be("https://idp.example.com/idp");
        }

        [TestMethod]
        public void KentorAuthServicesAuthenticationOptions_Ctor_IgnoresConfiguration()
        {
            var subject = new KentorAuthServicesAuthenticationOptions(loadConfiguration: false);

            subject.SPOptions.Should().BeNull();
            subject.IdentityProviders.Should().BeEmpty();
        }
    }
}
