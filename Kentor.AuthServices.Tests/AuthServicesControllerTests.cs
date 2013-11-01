using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Web.Mvc;
using Kentor.AuthServices.Mvc;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class AuthServicesControllerTests
    {
        [TestMethod]
        public void AuthServicesController_SignIn_Returns_SignIn()
        {
            var subject = new AuthServicesController().SignIn();

            subject.Should().BeOfType<RedirectResult>().And
                .Subject.As<RedirectResult>().Url.ToString()
                .Should().Contain("?SAMLRequest");
        }
    }
}
