using System;
using System.Collections.Specialized;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Configuration;
using FluentAssertions;
using Kentor.AuthServices.WebSso;
using NSubstitute;
using Kentor.AuthServices.TestHelpers;

namespace Kentor.AuthServices.Tests.WebSso
{
    [TestClass]
    public class AuthServicesUrlsTests
    {
        [TestMethod]
        public void AuthServicesUrls_Ctor_NullCheckRequest()
        {
            Action a = () => new AuthServicesUrls(null, new Options(new SPOptions()));

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("request"); ;
        }

        [TestMethod]
        public void AuthServicesUrls_Ctor_NullCheckOptions()
        {
            Action a = () => new AuthServicesUrls(
                new HttpRequestData("GET", new Uri("http://localhost")),
                null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
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
            Action a = () => new AuthServicesUrls(new Uri("http://localhost"), modulePath: null);

            a.ShouldThrow<ArgumentNullException>("modulePath");
        }

        [TestMethod]
        public void AuthServiecsUrls_Ctor_HandlesApplicationNotInRoot()
        {
            var appUrl = new Uri("http://localhost:73/SomePath");
            var modulePath = "/modulePath";

            var subject = new AuthServicesUrls(appUrl, modulePath);

            subject.AssertionConsumerServiceUrl.Should().Be(new Uri("http://localhost:73/SomePath/modulePath/Acs"));
            subject.SignInUrl.Should().Be(new Uri("http://localhost:73/SomePath/modulePath/SignIn"));
        }

        [TestMethod]
        public void AuthServicesUrls_Ctor_HandlesApplicationInRoot()
        {
            var appUrl = new Uri("http://localhost:42/");
            var modulePath = "/modulePath";

            var subject = new AuthServicesUrls(appUrl, modulePath);

            subject.AssertionConsumerServiceUrl.Should().Be(new Uri("http://localhost:42/modulePath/Acs"));
            subject.SignInUrl.Should().Be(new Uri("http://localhost:42/modulePath/SignIn"));
        }

        [TestMethod]
        public void AuthServicesUrls_Ctor_ChecksModulePathStartsWithSlash()
        {
            var appUrl = new Uri("http://localhost:42");
            var modulePath = "modulePath";

            Action a = () => new AuthServicesUrls(appUrl, modulePath);

            a.ShouldThrow<ArgumentException>("modulePath should start with /.");
        }

        [TestMethod]
        public void AuthServicesUrls_Ctor_EnsuresApplicationUrlEndsWithSlash()
        {
            var request = new HttpRequestData(
                "GET",
                new Uri("http://localhost:1234/Foo/Bar"),
                "/Foo",
                null,
                null,
                null);

            var options = StubFactory.CreateOptions();
            var subject = new AuthServicesUrls(request, options);
            subject.ApplicationUrl.OriginalString.Should().EndWith("/");
        }

        [TestMethod]
        public void AuthServiecsUrls_Ctor_AcceptsFullUrls()
        {
            var acsUrl = new Uri("http://localhost:73/MyApp/MyAcs");
            var signinUrl = new Uri("http://localhost:73/MyApp/MySignin");
            var appUrl = new Uri("http://localhost:73/MyApp");

            var subject = new AuthServicesUrls(acsUrl, signinUrl, appUrl);

            subject.AssertionConsumerServiceUrl.ToString().Should().Be(acsUrl.ToString());
            subject.SignInUrl.ToString().Should().Be(signinUrl.ToString());
            subject.ApplicationUrl.Should().Be(appUrl.ToString());
        }

        [TestMethod]
        public void AuthServicesUrls_Ctor_AllowsNullAcs()
        {
            // AssertionConsumerServiceURL is optional in the SAML spec 
            var subject = new AuthServicesUrls(null, new Uri("http://localhost/signin"), null);

            subject.AssertionConsumerServiceUrl.Should().Be(null);
            subject.SignInUrl.ToString().Should().Be("http://localhost/signin");
        }

        [TestMethod]
        public void AuthServicesUrls_Ctor_NullCheckSignin()
        {
            Action a = () => new AuthServicesUrls(
                new Uri("http://localhost/signin"),
                signInUrl: null,
                applicationUrl: new Uri("http://localhost"));

            a.ShouldThrow<ArgumentNullException>("signInUrl");
        }

        [TestMethod]
        public void AuthServicesUrls_Ctor_PerRequest_PublicOrigin()
        {
            var options = StubFactory.CreateOptionsPublicOrigin(new Uri("https://my.public.origin:8443/"));
            options.Notifications.GetPublicOrigin = (requestData) =>
            {
                return new Uri("https://special.public.origin/");
            };
            var urls = new AuthServicesUrls(new HttpRequestData("get", new Uri("http://servername/")), options);
            urls.AssertionConsumerServiceUrl.ShouldBeEquivalentTo("https://special.public.origin/AuthServices/Acs");
            urls.SignInUrl.ShouldBeEquivalentTo("https://special.public.origin/AuthServices/SignIn");
        }
    }
}
