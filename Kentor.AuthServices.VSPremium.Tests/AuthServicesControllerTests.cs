using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes;
using NSubstitute;
using System.IdentityModel.Services;
using Kentor.AuthServices.Mvc;
using FluentAssertions;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;

namespace Kentor.AuthServices.VSPremium.Tests
{
    [TestClass]
    public class AuthServicesControllerTests
    {
        [TestMethod]
        public void AuthServicesController_SignOut()
        {
            using(ShimsContext.Create())
            {
                var substituteSessionAuthModule = Substitute.For<SessionAuthenticationModule>();

                System.IdentityModel.Services.Fakes.ShimFederatedAuthentication.SessionAuthenticationModuleGet =
                    () => substituteSessionAuthModule;

                var substituteRequestContext = Substitute.For<RequestContext>();
                substituteRequestContext.HttpContext = Substitute.For<HttpContextBase>();
                substituteRequestContext.HttpContext.Request.Returns(Substitute.For<HttpRequestBase>());
                substituteRequestContext.HttpContext.Request.ApplicationPath.Returns("/path");

                var subject = new AuthServicesController()
                    {
                        Url = new UrlHelper(substituteRequestContext)
                    }.SignOut();

                subject.Should().BeOfType<RedirectResult>().And
                    .Subject.As<RedirectResult>().Url.Should().Be("/path/");

                substituteSessionAuthModule.Received().SignOut();
            }
        }
    }
}
