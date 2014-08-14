using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Owin;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class KentorAuthServicesAuthenticationOptionsTests
    {
        [TestMethod]
        public void KentorAuthServicesAuthenticationOptions_SetsDefaultDescriptionCaption()
        {
            var subject = new KentorAuthServicesAuthenticationOptions();

            subject.Description.Caption.Should().Be(Constants.DefaultAuthenticationType);
        }
    }
}
