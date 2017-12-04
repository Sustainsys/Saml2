using FluentAssertions;
using Kentor.AuthServices.TestHelpers;
using Kentor.AuthServices.WebSso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Specialized;
using System.Web;

namespace Kentor.AuthServices.HttpModule.Tests
{
    [TestClass]
    public class AuthServicesUrlsTests
    {
        [TestMethod]
        public void AuthServicesUrls_Ctor_FromHttpRequest_PublicOrigin()
        {
            var url = new Uri("http://example.com:42/ApplicationPath/Path?name=DROP%20TABLE%20STUDENTS");
            string appPath = "/ApplicationPath";
            var request = Substitute.For<HttpRequestBase>();
            request.HttpMethod.Returns("GET");
            request.Url.Returns(url);
            request.Form.Returns(new NameValueCollection { { "Key", "Value" } });
            request.ApplicationPath.Returns(appPath);
            var options = StubFactory.CreateOptionsPublicOrigin(new Uri("https://my.public.origin:8443/OtherPath"));
            var subject = request.ToHttpRequestData();
            var urls = new AuthServicesUrls(subject, options);
            urls.AssertionConsumerServiceUrl.ShouldBeEquivalentTo("https://my.public.origin:8443/OtherPath/AuthServices/Acs");
            urls.SignInUrl.ShouldBeEquivalentTo("https://my.public.origin:8443/OtherPath/AuthServices/SignIn");
        }
    }
}
