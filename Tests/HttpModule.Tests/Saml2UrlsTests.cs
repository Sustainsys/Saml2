using FluentAssertions;
using Sustainsys.Saml2.TestHelpers;
using Sustainsys.Saml2.WebSso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Specialized;
using System.Web;

namespace Sustainsys.Saml2.HttpModule.Tests
{
    [TestClass]
    public class Saml2UrlsTests
    {
        [TestMethod]
        public void Saml2Urls_Ctor_FromHttpRequest_PublicOrigin()
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
            var urls = new Saml2Urls(subject, options);
            urls.AssertionConsumerServiceUrl.Should().BeEquivalentTo(
				new Uri("https://my.public.origin:8443/OtherPath/Saml2/Acs"));
            urls.SignInUrl.Should().BeEquivalentTo(
				new Uri("https://my.public.origin:8443/OtherPath/Saml2/SignIn"));
        }
    }
}
