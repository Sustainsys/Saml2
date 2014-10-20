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
    }
}
