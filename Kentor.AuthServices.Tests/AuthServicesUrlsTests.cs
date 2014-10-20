using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Configuration;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class AuthServicesUrlsTests
    {
        [TestMethod]
        public void AuthServicesUrls_Ctor_NullCheckRequest()
        {
            Action a = () => new AuthServicesUrls(null, new SPOptions());

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("request"); ;
        }

        [TestMethod]
        public void AuthServicesUrls_Ctor_NullCheckOptions()
        {
            Action a = () => new AuthServicesUrls(
                new HttpRequestData("GET", new Uri("http://localhost")),
                null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("spOptions");
        }

        [TestMethod]
        public void AuthServicesUrls_Ctor_NullCheckApplicationUrl()
        {
            Action a = () => new AuthServicesUrls(null, "modulePath");

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("applicationUrl");
        }

        [TestMethod]
        public void AuthServicesUrls_Ctor_NullCheckModulePath()
        {
            Action a = () => new AuthServicesUrls(new Uri("http://localhost"), null);

            a.ShouldThrow<ArgumentNullException>("modulePath");
        }

        [TestMethod]
        public void AuthServiecsUrls_AssertionConsumerServiceUrl_HandlesApplicationNotInRoot()
        {
            var appUrl = new Uri("http://localhost:73/SomePath");
            var modulePath = "/modulePath";

            var subject = new AuthServicesUrls(appUrl, modulePath);

            subject.AssertionConsumerServiceUrl.Should().Be(new Uri("http://localhost:73/SomePath/modulePath/Acs"));
        }

        [TestMethod]
        public void AuthServicesUrls_AssertionConsumerServiceUrl_HandlesApplicationInRoot()
        {
            var appUrl = new Uri("http://localhost:42/");
            var modulePath = "/modulePath";

            var subject = new AuthServicesUrls(appUrl, modulePath);

            subject.AssertionConsumerServiceUrl.Should().Be(new Uri("http://localhost:42/modulePath/Acs"));
        }

        [TestMethod]
        public void AuthServicesUrls_Ctor_ChecksModulePathStartsWithSlash()
        {
            var appUrl = new Uri("http://localhost:42");
            var modulePath = "modulePath";

            Action a = () => new AuthServicesUrls(appUrl, modulePath);

            a.ShouldThrow<ArgumentException>("modulePath should start with /.");
        }
    }
}
