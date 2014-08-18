using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Owin;
using FluentAssertions;
using Microsoft.Owin.Security;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class KentorAuthServicesAuthenticationOptionsTests
    {
        [TestMethod]
        public void KentorAuthServicesAuthenticationOptions_SetsDefault()
        {
            var subject = new KentorAuthServicesAuthenticationOptions();

            subject.Description.Caption.Should().Be(Constants.DefaultCaption);
            subject.AuthenticationMode.Should().Be(AuthenticationMode.Passive);
            subject.MetadataPath.ToString().Should().Be("/AuthServices");
        }
    }
}
